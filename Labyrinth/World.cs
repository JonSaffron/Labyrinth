using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.DataStructures;
using Labyrinth.ClockEvents;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.Messages;
using Labyrinth.Services.PathFinder;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public class World
        {
        private readonly TilePos _worldSize;
        private Tile[,] _tiles;
        private readonly bool _restartInSameRoom;
        private readonly Dictionary<int, PlayerStartState> _playerStartStates;
        private readonly Player _player;
        private readonly WorldClock _worldClock;
        private readonly WorldWindow _worldWindow;
        private bool _applyEndOfWorldEffect;
        private Effect? _endOfWorldEffect;
        private WorldState _worldState = WorldState.Loading;

        public readonly GameState GameState;

        public World(IWorldLoader worldLoader)
            {
            if (worldLoader == null) 
                throw new ArgumentNullException(nameof(worldLoader));

            this._worldSize = worldLoader.WorldSize;
            this._tiles = worldLoader.FloorTiles;
            this._restartInSameRoom = worldLoader.RestartInSameRoom;
            this._playerStartStates = worldLoader.PlayerStartStates;

            var gameObjectCollection = new GameObjectCollection(worldLoader.WorldSize);
            this.GameState = new GameState(gameObjectCollection);
            worldLoader.AddGameObjects(this.GameState);
            ValidateGameState(this.GameState);
            this._player = GameState.Player;

            this._worldClock = new WorldClock();
            if (worldLoader.UnlockLevels)
                {
                this._worldClock.AddEventHandler(new UnlockLevel(this));
                }

            foreach (var dist in worldLoader.FruitDistributions.Where(item => item.PopulationMethod.WillReplenish()))
                {
                var replenishFruit = new ReplenishFruit(dist, this.GameState);
                this._worldClock.AddEventHandler(replenishFruit);
                }

            this._worldWindow = new WorldWindow();
            GlobalServices.SetCentrePointProvider(this._worldWindow);
            
            Messenger.Default.Register<WorldCompleted>(this, WorldCompleted);
            Messenger.Default.Register<LifeLost>(this, LostLife);
            }

        public void LoadContent(ContentManager contentManager)
            {
            this._endOfWorldEffect = contentManager.Load<Effect>("Effects/EndOfWorldFlash");
            }

        private void ValidateGameState(GameState gameState)
            {
            var msg = new WorldLoaderProgress("Validating gamestate");
            Messenger.Default.Send(msg);

            var tcv = new TileContentValidator();
            var issues = new List<string>();
            var cy = this._worldSize.Y;
            var cx = this._worldSize.X;
            for (int y = 0; y < cy; y++)
                {
                for (int x = 0; x < cx; x++)
                    {
                    var tp = new TilePos(x, y);
                    var items = gameState.GetItemsOnTile(tp).ToList();
                    if (!tcv.IsListOfObjectsValid(items, out string reason))
                        issues.Add($"{tp}: {reason}");
                    }
                }

            if (issues.Count != 0)
                {
                var message = string.Join("\r\n", issues);
                throw new InvalidOperationException(message);
                }
            }

        public void ResetWorldAfterLosingLife()
            {
            GlobalServices.GameState.RemoveBangsAndShots();

            var worldAreaId = this.GetWorldAreaIdForTilePos(this._player.TilePosition);
            var pss = this._playerStartStates[worldAreaId];
            var resetPosition = this._restartInSameRoom ? GetRestartLocation(this._player.TilePosition, pss.Position) : pss.Position;
            this._player.ResetPositionAndEnergy(resetPosition.ToPosition(), pss.Energy);

            ResetWorldForStartingNewLife();
            }

        private static TilePos GetRestartLocation(TilePos lastPosition, TilePos levelStartPosition)
            {
            var topLeft = GetContainingRoom(lastPosition).TopLeft;
            var startPos = new TilePos(topLeft.X + 8, topLeft.Y + 5);
            if (!GetFreeTileWithinRoom(startPos, out var tilePos))
                {
                if (!GetFreeTileWithinRoom(levelStartPosition, out tilePos))
                    throw new InvalidOperationException("Could not find a free tile to put the player on.");
                }
            return tilePos;
            }

        public void ResetWorldForStartingNewLife()
            {
            Point roomStart = GetContainingRoom(this._player.Position).Location;
            this._worldWindow.ResetPosition(new Vector2(roomStart.X, roomStart.Y));
            this._player.Reset();

            MoveNearbyMonstersToASafeDistance();

            GlobalServices.SoundPlayer.Play(GameSound.PlayerStartsNewLife);
            this._worldState = WorldState.Normal;
            GlobalServices.PlayerInput.Enabled = true;
            }

        private void MoveNearbyMonstersToASafeDistance()
            {
            var monsters = GetNearbyMonsters();
            foreach (var monster in monsters)
                MoveMonsterToSafeDistance(monster);
            }

        private IEnumerable<Monster> GetNearbyMonsters()
            {
            var area = new TileRect(new TilePos(this._player.TilePosition.X - 16, this._player.TilePosition.Y - 16), 32, 32);
            var searchParameters = new SearchParameters
                {
                EndLocation = this._player.TilePosition,
                MaximumLengthOfPath = 20,
                CanBeOccupied = tp => !GlobalServices.GameState.IsImpassableItemOnTile(tp)
                };

            var result = new List<Monster>();
            foreach (var monsterOrEgg in GlobalServices.GameState.AllItemsInRectangle(area).OfType<IMonster>())
                {
                var monster = monsterOrEgg as Monster;
                if (monsterOrEgg is MonsterEgg egg)
                    {
                    monster = egg.HatchEgg();
                    }

                // look at any non-stationary monsters including those that aren't active
                if (monster != null && monster.Mobility != MonsterMobility.Stationary)
                    {
                    searchParameters.StartLocation = monster.TilePosition;

                    var pf = new PathFinder(searchParameters);
                    if (pf.TryFindPath(out _))
                        result.Add(monster);
                    }
                }

            return result;
            }

        private void MoveMonsterToSafeDistance(Monster monster)
            {
            var repelParameters = new RepelParameters
                {
                StartLocation = monster.TilePosition,
                RepelLocation = _player.TilePosition,
                CanBeOccupied = tp => !GlobalServices.GameState.IsImpassableItemOnTile(tp),
                MaximumLengthOfPath = 24,
                MinimumDistanceToMoveAway = 16
                };
            var repeller = new RepelObject(repelParameters);
            var result = repeller.TryFindPath(out var path);
            if (result && path!.Any())
                {
                monster.ResetPosition(path!.Last().ToPosition());
                }
            }

        /// <summary>
        /// Updates the world in accordance to how much time has passed
        /// </summary>
        public WorldState Update(GameTime gameTime)
            {
            this._worldWindow.RecalculateWindow(gameTime);
            this._worldClock.Update(gameTime);
            
            UpdateGameItems(gameTime);
            
            return this._worldState;
            }

        private void UpdateGameItems(GameTime gameTime)
            {
            //const float minimumDistance = Constants.TileLength * 2;

            //int countOfGameItemsThatMoved = 0;
            //int countOfGameItems = 0;
            //int countOfInteractions = 0;
            foreach (IGameObject currentItem in GlobalServices.GameState.GetSurvivingGameObjects())
                {
                //countOfGameItems++;

                var movingItem = currentItem as IMovingItem;
                if (!currentItem.Update(gameTime) || movingItem == null)
                    continue;
                GlobalServices.GameState.UpdatePosition(movingItem);
                //countOfGameItemsThatMoved++;

                Rectangle? bounds = null;
                var tileRect = currentItem.TilePosition.GetRectAroundPosition(1);
                foreach (var si in GlobalServices.GameState.AllItemsInRectangle(tileRect))
                    {
                    if (currentItem == si)
                        continue;   

                    //if (Math.Abs(Vector2.DistanceSquared(currentItem.Position, si.Position)) <= minimumDistance)
                        {
                        bounds ??= currentItem.BoundingRectangle;
                        if (bounds.Value.Intersects(si.BoundingRectangle))
                            {
                            //countOfInteractions++;
                            //Trace.WriteLine(string.Format("interacting {0} and {1}", currentItem, si));
                            var interaction = BuildInteraction(movingItem, si);
                            interaction.Collide();
                            }
                        }
                    }
                }
            //if (countOfGameItemsThatMoved > 0)
            //    Trace.WriteLine(string.Format("Total interactive items: {0}, those that moved: {1}, interactions: {2}", countOfGameItems, countOfGameItemsThatMoved, countOfInteractions));
            }

        /// <summary>
        /// Builds an interaction object from the two specified game objects
        /// </summary>
        /// <param name="updatedItem">An object that has just moved position</param>
        /// <param name="actedUponItem">An object whose position overlaps the first object</param>
        /// <returns>An instance of an interaction object</returns>
        private static IInteraction BuildInteraction(IMovingItem updatedItem, IGameObject actedUponItem)
            {
            var result = actedUponItem is IMovingItem secondMovingItem
                ? (IInteraction) new InteractionWithMovingItems(updatedItem, secondMovingItem)
                : new InteractionWithStaticItems(actedUponItem, updatedItem);
            return result;
            }

        /// <summary>
        /// Gets the rectangle that encompasses the room that contains the specified position
        /// </summary>
        /// <param name="position">Specifies the position within the world</param>
        /// <returns>A rectangular structure which specifies the co-ordinates of the room containing the specified position</returns>
        public static Rectangle GetContainingRoom(Vector2 position)
            {
            var roomX = (int) (position.X / Constants.RoomSizeInPixels.X);
            var roomY = (int) (position.Y / Constants.RoomSizeInPixels.Y);
            var r = new Rectangle(roomX * (int) Constants.RoomSizeInPixels.X, roomY * (int) Constants.RoomSizeInPixels.Y, (int) Constants.RoomSizeInPixels.X, (int) Constants.RoomSizeInPixels.Y);
            return r;
            }

        /// <summary>
        /// Gets the rectangle that encompasses the room that contains the specified position
        /// </summary>
        /// <param name="position">Specifies the position within the world</param>
        /// <returns>A rectangular structure which specifies the co-ordinates of the room containing the specified position</returns>
        public static TileRect GetContainingRoom(TilePos position)
            {
            var roomX = (int)(position.X / Constants.RoomSizeInTiles.X);
            var roomY = (int)(position.Y / Constants.RoomSizeInTiles.Y);
            var topLeft = new TilePos(roomX * (int) Constants.RoomSizeInTiles.X, roomY * (int) Constants.RoomSizeInTiles.Y);
            var result = new TileRect(topLeft, (int) Constants.RoomSizeInTiles.X, (int) Constants.RoomSizeInTiles.Y);
            return result;
            }

        /// <summary>
        /// Draw everything in the WorldToLoad from background to foreground.
        /// </summary>
        public void Draw(ISpriteBatch spriteBatch)
            {
            var windowPosition = this._worldWindow.WindowPosition;
            spriteBatch.WindowPosition = this._worldWindow.WindowPosition;
            var tileRectangle = GetRectangleEnclosingTilesThatAreCurrentlyInView(windowPosition);

            spriteBatch.Begin(this._applyEndOfWorldEffect ? this._endOfWorldEffect : null);
            WorldRenderer worldRenderer = new WorldRenderer(tileRectangle, spriteBatch, GlobalServices.SpriteLibrary);
            worldRenderer.RenderFloorTiles(ref this._tiles);
            worldRenderer.RenderWorld();
            spriteBatch.End();
            }

        private static TileRect GetRectangleEnclosingTilesThatAreCurrentlyInView(Vector2 windowOffset)
            {
            var roomStartX = (int)Math.Floor(windowOffset.X / Constants.TileLength);
            var roomStartY = (int)Math.Floor(windowOffset.Y / Constants.TileLength);
            
            var roomEndX = (int)Math.Ceiling((windowOffset.X + Constants.RoomSizeInPixels.X) / Constants.TileLength);
            var roomEndY = (int)Math.Ceiling((windowOffset.Y + Constants.RoomSizeInPixels.Y) / Constants.TileLength);

            var result = new TileRect(new TilePos(roomStartX, roomStartY), roomEndX - roomStartX, roomEndY - roomStartY);
            Debug.Assert(result.Width == (int) Constants.RoomSizeInTiles.X || result.Width == ((int) Constants.RoomSizeInTiles.X + 1));
            Debug.Assert(result.Height == (int) Constants.RoomSizeInTiles.Y || result.Height == ((int) Constants.RoomSizeInTiles.Y + 1));
            return result;
            }

        public void MoveUpALevel()
            {
            if (!this._player.IsAlive())
                return;

            int currentWorldAreaId = this.GetWorldAreaIdForTilePos(this._player.TilePosition);
            var newState = this._playerStartStates.Values.Where(item => item.Id > currentWorldAreaId).MinBy(item => item.Id);
            if (newState == null)
                return;

            var crystals = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Where(c => this.GetWorldAreaIdForTilePos(c.TilePosition) == currentWorldAreaId);
            foreach (var c in crystals)
                {
                var i = new InteractionWithStaticItems(c, this._player);
                i.Collide();
                }
            this._player.ResetPositionAndEnergy(newState.Position.ToPosition(), newState.Energy);
            GlobalServices.GameState.UpdatePosition(this._player);
            var boulder = GlobalServices.GameState.DistinctItemsOfType<Boulder>().FirstOrDefault();
            if (boulder != null)
                {
                var startPos = new TilePos(newState.Position.X + 2, newState.Position.Y);
                if (GetFreeTileWithinRoom(startPos, out var boulderPosition))
                    {
                    boulder.ResetPosition(boulderPosition.ToPosition());
                    }
                }
            Point roomStart = GetContainingRoom(this._player.Position).Location;
            this._worldWindow.ResetPosition(new Vector2(roomStart.X, roomStart.Y));
            }

        private static bool GetFreeTileWithinRoom(TilePos startPos, out TilePos tilePos)
            {
            var roomSize = (int) (Constants.RoomSizeInTiles.X * Constants.RoomSizeInTiles.Y);
            var roomDimensions = GetContainingRoom(startPos);
            for (int i = 1; i <= roomSize; i++)
                {
                var positionInSpiral = GetPositionInSpiral(i);
                var potentialPosition = new TilePos(startPos.X + positionInSpiral.X, startPos.Y + positionInSpiral.Y);
                if (GetContainingRoom(potentialPosition).TopLeft == roomDimensions.TopLeft && !GlobalServices.GameState.GetItemsOnTile(potentialPosition).Any())
                    {
                    tilePos = potentialPosition;
                    return true;
                    }
                }
            tilePos = new TilePos();
            return false;
            }

        private static Point GetPositionInSpiral(int n)
            {
            var k = (int) Math.Ceiling((Math.Sqrt(n) - 1) / 2);
            var t = 2 * k + 1;
            var m = (int) Math.Pow(t, 2);

            t -= 1;

            if (n >= m - t)
                {
                return new Point(k - (m - n), -k);
                }

            m -= t;

            if (n >= m - t)
                {
                return new Point(-k, -k + (m - n));
                }

            m -= t;

            if (n >= m - t)
                {
                return new Point(-k + (m - n), k);
                }

            return new Point(k, k - (m - n - t));
            }
            
        public int GetWorldAreaIdForTilePos(TilePos tp)
            {
            var result = this._tiles[tp.X, tp.Y].WorldAreaId;
            return result;
            }

        private void LostLife(LifeLost lifeLost)
            {
            GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerDies, 
                (sender, args) => this._worldState = WorldState.LostLife);
            }

        /// <summary>
        /// This gets called as soon as the player collects the final crystal, but the world status will only change once the end world sound has finished playing
        /// </summary>
        /// <param name="worldCompleted">The WorldCompleted message</param>
        private void WorldCompleted(WorldCompleted worldCompleted)
            {
            foreach (IMonster monster in GlobalServices.GameState.DistinctItemsOfType<IMonster>())
                {
                monster.InstantlyExpire();
                }

            GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerFinishesWorld,
                (sender, args) => this._worldState = WorldState.FinishedWorld);

            // TODO: also need to prevent player energy going down
            GlobalServices.PlayerInput.Enabled = false;

            GlobalServices.Game.Components.Add(new FlashEffectTimer(this));
            }

        private class FlashEffectTimer : GameComponent
            {
            private double _timeElapsed;
            private bool _flashState;
            private const double SecondsToShowEachColour = 0.080d;
            private readonly World _world;

            public FlashEffectTimer(World world) : base(GlobalServices.Game)
                {
                this._world = world ?? throw new ArgumentNullException(nameof(world));
                this._world._applyEndOfWorldEffect = true;
                }

            public override void Update(GameTime gameTime)
                {
                var effect = this._world._endOfWorldEffect;
                if (effect == null)
                    throw new InvalidOperationException("EndOfWorldEffect resource has not been loaded.");

                this._timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
                while (this._timeElapsed >= SecondsToShowEachColour)
                    {
                    this._timeElapsed -= SecondsToShowEachColour;
                    this._flashState = !this._flashState;
                    }

                effect.Parameters["flashOn"].SetValue(this._flashState);
                }
            }
        }
    }
