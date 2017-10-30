using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Labyrinth.GameObjects;
using Labyrinth.Services.PathFinder;

namespace Labyrinth
    {
    public class World : ICentrePointProvider
        {
        private readonly Vector2 _centreOfRoom = Constants.RoomSizeInPixels / 2.0f;

        private int _gameClock;
        private double _time;
        private int _levelUnlocked;
        private LevelReturnType _levelReturnType = LevelReturnType.Normal;
        private bool _doNotUpdate;
        private readonly bool _restartInSameRoom;
        
        [NotNull] public readonly Player Player;

        [NotNull] private readonly Tile[,] _tiles;
        [NotNull] private readonly Dictionary<int, PlayerStartState> _playerStartStates;
        [NotNull] private readonly List<StaticItem>[] _itemsToDrawByZOrder;

        public Vector2 WindowPosition { get; private set; }

        public World(IWorldLoader worldLoader)
            {
            this._tiles = worldLoader.GetFloorTiles();
            this._restartInSameRoom = worldLoader.RestartInSameRoom;
            var gameObjectCollection = new GameObjectCollection();
            this._playerStartStates = worldLoader.GetPlayerStartStates();
            var gameState = new GameState(gameObjectCollection);
            worldLoader.GetGameObjects(gameState);
            this.Player = gameState.Player;
            GlobalServices.SetGameState(gameState);

            this._itemsToDrawByZOrder = new List<StaticItem>[10];
            for (int i = 0; i < this._itemsToDrawByZOrder.GetLength(0); i++)
                this._itemsToDrawByZOrder[i] = new List<StaticItem>();
            }

        public void ResetLevelAfterLosingLife(ISpriteBatch spw)
            {
            GlobalServices.GameState.RemoveBangsAndShots();

            var worldAreaId = this.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            var pss = this._playerStartStates[worldAreaId];
            var resetPosition = this._restartInSameRoom ? this.Player.TilePosition : pss.Position;
            this.Player.ResetPositionAndEnergy(resetPosition.ToPosition(), pss.Energy);

            ResetLevelForStartingNewLife(spw);
            }
        
        public void ResetLevelForStartingNewLife(ISpriteBatch spw)
            {
            if (spw != null)
                {
                Point roomStart = GetContainingRoom(this.Player.Position).Location;
                this.WindowPosition = new Vector2(roomStart.X, roomStart.Y);
                }
            this.Player.Reset();

            MoveNearbyMonstersToASafeDistance();

            GlobalServices.SoundPlayer.Play(GameSound.PlayerStartsNewLife);
            this._levelReturnType = LevelReturnType.Normal;
            _doNotUpdate = false;
            }

        private void MoveNearbyMonstersToASafeDistance()
            {
            var monsters = GetNearbyMonsters();
            foreach (var monster in monsters)
                MoveMonsterToSafeDistance(monster);
            }

        private IEnumerable<Monster> GetNearbyMonsters()
            {
            var area = new TileRect(new TilePos(this.Player.TilePosition.X - 16, this.Player.TilePosition.Y - 16), 32, 32);
            var searchParameters = new SearchParameters
                {
                EndLocation = this.Player.TilePosition,
                MaximumLengthOfPath = 20,
                CanBeOccupied = tp => !GlobalServices.GameState.IsImpassableItemOnTile(tp)
                };

            var result = new List<Monster>();
            foreach (var monster in GlobalServices.GameState.AllItemsInRectangle(area).OfType<Monster>())
                {
                if (!monster.IsStill)
                    {
                    searchParameters.StartLocation = monster.TilePosition;

                    var pf = new PathFinder(searchParameters);
                    // ReSharper disable once NotAccessedVariable
                    IList<TilePos> path;
                    if (pf.TryFindPath(out path))
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
                RepelLocation = Player.TilePosition,
                CanBeOccupied = tp => !GlobalServices.GameState.IsImpassableItemOnTile(tp),
                MaximumLengthOfPath = 24,
                MinimumDistanceToMoveAway = 12
                };
            var repeller = new RepelObject(repelParameters);
            var result = repeller.TryFindPath(out var path);
            if (result && path.Any())
                {
                monster.ResetPosition(path.Last().ToPosition());
                }
            }

        public void SetLevelReturnType(LevelReturnType levelReturnType)
            {
            this._levelReturnType = levelReturnType;
            this._doNotUpdate = true;
            }

        public void SetDoNotUpdate()
            {
            this._doNotUpdate = true;
            }

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public LevelReturnType Update(GameTime gameTime)
            {
            if (this._doNotUpdate)
                return this._levelReturnType;
            
            UpdateGameClock(gameTime);
            
            UpdateGameItems(gameTime);
            
            return this._levelReturnType;
            }

        private void UpdateGameClock(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time > Constants.GameClockResolution)
                {
                this._time -= Constants.GameClockResolution;
                if (this._gameClock < int.MaxValue)
                    this._gameClock++; 

                int levelToUnlock = this._gameClock >> 13;
                while (this._levelUnlocked < levelToUnlock)
                    {
                    this._levelUnlocked++;
                    UnlockLevel(this._levelUnlocked);
                    }
                }
            }

        private void UnlockLevel(int levelThatPlayerShouldHaveReached)
            {
            foreach (Monster m in GlobalServices.GameState.DistinctItemsOfType<Monster>())
                {
                if (m.IsEgg || m.IsActive || !m.IsExtant || m.ChangeRooms == ChangeRooms.StaysWithinRoom)
                    continue;
                
                int worldAreaId = this.GetWorldAreaIdForTilePos(m.TilePosition);
                if (worldAreaId < levelThatPlayerShouldHaveReached)
                    m.IsActive = true;
                }
            }

        private void UpdateGameItems(GameTime gameTime)
            {
            //const float minimumDistance = (Constants.TileLength * 2;

            int countOfGameItemsThatMoved = 0;
            int countOfGameItems = 0;
            int countOfInteractions = 0;
            foreach (var currentItem in GlobalServices.GameState.GetSurvivingInteractiveItems())
                {
                countOfGameItems++;

                if (!currentItem.Update(gameTime))
                    continue;
                GlobalServices.GameState.UpdatePosition(currentItem);
                countOfGameItemsThatMoved++;

                Rectangle? bounds = null;
                var tileRect = currentItem.TilePosition.GetRectAroundPosition(1);
                foreach (var si in GlobalServices.GameState.AllItemsInRectangle(tileRect))
                    {
                    if (currentItem == si)
                        continue;   

                    //if (Math.Abs(Vector2.DistanceSquared(currentItem.Position, si.Position)) <= minimumDistance)
                        {
                        if (bounds == null)
                            {
                            Trace.WriteLine(string.Format("checking {0} and {1}", currentItem, si));
                            bounds = currentItem.BoundingRectangle;
                            }
                        if (bounds.Value.Intersects(si.BoundingRectangle))
                            {
                            countOfInteractions++;
                            Trace.WriteLine(string.Format("interacting {0} and {1}", currentItem, si));
                            var interaction = BuildInteraction(currentItem, si);
                            interaction?.Collide();
                            }
                        }
                    }
                }
            if (countOfGameItemsThatMoved > 0)
                Trace.WriteLine(string.Format("Total interactive items: {0}, those that moved: {1}, interactions: {2}", countOfGameItems, countOfGameItemsThatMoved, countOfInteractions));
            }

        [CanBeNull]
        private IInteraction BuildInteraction(StaticItem thisGameItem, StaticItem thatGameItem)
            {
            var items = new[] { thisGameItem, thatGameItem };
            var staticItem = items.FirstOrDefault(item => !(item is MovingItem));
            if (staticItem == null)
                {
                IInteraction result = new InteractionWithMovingItems((MovingItem) thisGameItem, (MovingItem) thatGameItem);
                return result;
                }
            
            var otherItem = items.Single(item => item != staticItem);
            var movingItem = otherItem as MovingItem;
            if (otherItem is MovingItem)
                {
                IInteraction result = new InteractionWithStaticItems(this, staticItem, movingItem);
                return result;
                }

            return null;
            }

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the World and fall off the bottom.
        /// </summary>
        public bool IsTileWithinWorld(TilePos tp)
            {
            var x = tp.X;
            var y = tp.Y;
            var result = !(x < 0 || x >= this._tiles.GetLength(0) || y < 0 || y >= this._tiles.GetLength(1));
            return result;
            }

        public static Rectangle GetContainingRoom(Vector2 position)
            {
            var roomx = (int) (position.X / Constants.RoomSizeInPixels.X);
            var roomy = (int) (position.Y / Constants.RoomSizeInPixels.Y);
            var r = new Rectangle(roomx * (int) Constants.RoomSizeInPixels.X, roomy * (int) Constants.RoomSizeInPixels.Y, (int) Constants.RoomSizeInPixels.X, (int) Constants.RoomSizeInPixels.Y);
            return r;
            }
        
        private void RecalculateWindow(GameTime gameTime)
            {
            const float currentVelocity = 750;
            
            var roomRectangle = GetContainingRoom(this.Player.Position);
            var movingTowards = new Vector2(roomRectangle.Left, roomRectangle.Top);
            Vector2 position = this.WindowPosition;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float remainingMovement = currentVelocity * elapsed;

            var distanceToDestination = Vector2.Distance(position, movingTowards);
            bool hasArrivedAtDestination = (distanceToDestination <= remainingMovement);
            if (hasArrivedAtDestination)
                {
                position = movingTowards;
                }
            else
                {
                var vectorBetweenPoints = movingTowards - position;
                var unitVectorOfTravel = Vector2.Normalize(vectorBetweenPoints);
                var displacement = unitVectorOfTravel * remainingMovement;
                position += displacement;
                }
            
            this.WindowPosition = position;
            }

        /// <summary>
        /// Draw everything in the World from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
            {
            RecalculateWindow(gameTime);
            
            var tileRect = GetRectangleEnclosingTilesThatAreCurrentlyInView(this.WindowPosition);
            DrawFloorTiles(spriteBatch, tileRect);
            
            var itemsToDraw = GlobalServices.GameState.AllItemsInRectangle(tileRect);
            foreach (var item in itemsToDraw)
                {
                int zOrder = item.DrawOrder;
                this._itemsToDrawByZOrder[zOrder].Add(item);
                }

            foreach (var list in this._itemsToDrawByZOrder)
                {
                foreach (var item in list)
                    item.Draw(gameTime, spriteBatch);
                list.Clear();
                }
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

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        private void DrawFloorTiles(ISpriteBatch spriteBatch, TileRect tr)
            {
            for (int j = 0; j < tr.Height; j++)
                {
                int y = tr.TopLeft.Y + j;

                for (int i = 0; i < tr.Width; i++)
                    {
                    int x = tr.TopLeft.X + i;

                    var tp = new TilePos(x, y);
                    Texture2D texture = this._tiles[tp.X, tp.Y].Floor;
                    if (texture != null)
                        {
                        Vector2 position = new Vector2(x, y) * Constants.TileSize;
                        spriteBatch.DrawEntireTextureInWindow(texture, position);
                        }
                    }
                }
            }

        public Vector2 CentrePoint
            {
            get
                {
                var result = this.WindowPosition + _centreOfRoom;
                return result;
                }
            }

        public void MoveUpALevel()
            {
            if (!this.Player.IsAlive())
                return;

            int currentWorldAreaId = this.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            int maxId = this._playerStartStates.Max(item => item.Key);
            var newState = Enumerable.Range(currentWorldAreaId + 1, maxId - currentWorldAreaId).Select(i => this._playerStartStates[i]).FirstOrDefault(startState => startState != null);
            if (newState == null)
                return;

            var crystals = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Where(c => this.GetWorldAreaIdForTilePos(c.TilePosition) == currentWorldAreaId);
            foreach (var c in crystals)
                {
                var i = new InteractionWithStaticItems(this, c, this.Player);
                i.Collide();
                }
            this.Player.ResetPositionAndEnergy(newState.Position.ToPosition(), newState.Energy);
            GlobalServices.GameState.UpdatePosition(this.Player);
            var boulder = GlobalServices.GameState.DistinctItemsOfType<Boulder>().FirstOrDefault();
            if (boulder != null)
                {
                TilePos? tp = null;
                for (int i = 0; i < 16; i++)
                    {
                    int x = (i % 2 == 0) ? 7 - (i / 2) : 8 + ((i - 1) / 2);
                    var potentialPosition = new TilePos(x, newState.Position.Y);
                    if (!GlobalServices.GameState.IsStaticItemOnTile(potentialPosition))
                        {
                        tp = potentialPosition;
                        break;
                        }
                    }
                if (tp.HasValue)
                    {
                    boulder.ResetPosition(tp.Value.ToPosition());
                    GlobalServices.GameState.UpdatePosition(boulder);
                    }
                }
            Point roomStart = GetContainingRoom(this.Player.Position).Location;
            this.WindowPosition = new Vector2(roomStart.X, roomStart.Y);
            }

        public int GetWorldAreaIdForTilePos(TilePos tp)
            {
            var result = this._tiles[tp.X, tp.Y].WorldAreaId;
            return result;
            }
        }
    }
