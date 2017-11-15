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
        
        private TilePos _worldSize;
        [NotNull] private readonly Tile[,] _tiles;
        private readonly bool _restartInSameRoom;
        [NotNull] private readonly Dictionary<int, PlayerStartState> _playerStartStates;
        [NotNull] public readonly Player Player;

        public Vector2 WindowPosition { get; private set; }

        public World([NotNull] IWorldLoader worldLoader, [NotNull] string level)
            {
            worldLoader.LoadWorld(level);
            this._worldSize = worldLoader.WorldSize;
            this._tiles = worldLoader.GetFloorTiles();
            this._restartInSameRoom = worldLoader.RestartInSameRoom;
            this._playerStartStates = worldLoader.GetPlayerStartStates();

            var gameObjectCollection = new GameObjectCollection();
            var gameState = new GameState(gameObjectCollection);
            GlobalServices.SetGameState(gameState);
            worldLoader.AddGameObjects(gameState);
            this.Player = gameState.Player;

            ValidateGameState(gameState);
            }

        private void ValidateGameState(GameState gameState)
            {
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
                        issues.Add(tp + ": " + reason);
                    }
                }

            if (issues.Count != 0)
                {
                var message = string.Join("\r\n", issues);
                throw new InvalidOperationException(message);
                }
            }

        public void ResetLevelAfterLosingLife()
            {
            GlobalServices.GameState.RemoveBangsAndShots();

            var worldAreaId = this.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            var pss = this._playerStartStates[worldAreaId];
            var resetPosition = this._restartInSameRoom ? GetRestartLocation(this.Player.TilePosition, pss.Position) : pss.Position;
            this.Player.ResetPositionAndEnergy(resetPosition.ToPosition(), pss.Energy);

            ResetLevelForStartingNewLife();
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

        public void ResetLevelForStartingNewLife()
            {
            Point roomStart = GetContainingRoom(this.Player.Position).Location;
            this.WindowPosition = new Vector2(roomStart.X, roomStart.Y);
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
                if (monster.IsEgg)
                    {
                    monster.EggHatches(monster, new EventArgs());
                    }
                if (!monster.IsStatic)
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
                MinimumDistanceToMoveAway = 16
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
            // todo in the 2nd world an acorn is randomly dropped every so often - 280e onwards

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
                            //Trace.WriteLine(string.Format("checking {0} and {1}", currentItem, si));
                            bounds = currentItem.BoundingRectangle;
                            }
                        if (bounds.Value.Intersects(si.BoundingRectangle))
                            {
                            countOfInteractions++;
                            //Trace.WriteLine(string.Format("interacting {0} and {1}", currentItem, si));
                            var interaction = BuildInteraction(currentItem, si);
                            interaction?.Collide();
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
        /// <param name="thatGameItem">An object whose position overlaps the first object</param>
        /// <returns>An instance of an interaction object</returns>
        private IInteraction BuildInteraction(MovingItem updatedItem, StaticItem thatGameItem)
            {
            var result = thatGameItem is MovingItem secondMovingItem
                ? (IInteraction) new InteractionWithMovingItems(updatedItem, secondMovingItem)
                : new InteractionWithStaticItems(this, thatGameItem, updatedItem);
            return result;
            }

        /// <summary>
        /// Gets whether the specified tile is within the world area
        /// </summary>
        public bool IsTileWithinWorld(TilePos tp)
            {
            return !(tp.X < 0 || tp.X >= this._worldSize.X || tp.Y < 0 || tp.Y >= this._worldSize.Y);
            }

        /// <summary>
        /// Gets the rectangle that encompasses the room that contains the specified position
        /// </summary>
        /// <param name="position">Specifies the position within the world</param>
        /// <returns>A rectangular structure which specifies the co-ordinates of the room containing the specified position</returns>
        public static Rectangle GetContainingRoom(Vector2 position)
            {
            var roomx = (int) (position.X / Constants.RoomSizeInPixels.X);
            var roomy = (int) (position.Y / Constants.RoomSizeInPixels.Y);
            var r = new Rectangle(roomx * (int) Constants.RoomSizeInPixels.X, roomy * (int) Constants.RoomSizeInPixels.Y, (int) Constants.RoomSizeInPixels.X, (int) Constants.RoomSizeInPixels.Y);
            return r;
            }

        /// <summary>
        /// Gets the rectangle that encompasses the room that contains the specified position
        /// </summary>
        /// <param name="position">Specifies the position within the world</param>
        /// <returns>A rectangular structure which specifies the co-ordinates of the room containing the specified position</returns>
        private static TileRect GetContainingRoom(TilePos position)
            {
            var roomx = (int)(position.X / Constants.RoomSizeInTiles.X);
            var roomy = (int)(position.Y / Constants.RoomSizeInTiles.Y);
            var topLeft = new TilePos(roomx * (int) Constants.RoomSizeInTiles.X, roomy * (int) Constants.RoomSizeInTiles.Y);
            var result = new TileRect(topLeft, (int) Constants.RoomSizeInTiles.X, (int) Constants.RoomSizeInTiles.Y);
            return result;
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
        public void Draw(GameTime gameTime, [NotNull] ISpriteBatch spriteBatch)
            {
            RecalculateWindow(gameTime);
            
            var tileRect = GetRectangleEnclosingTilesThatAreCurrentlyInView(this.WindowPosition);
            DrawFloorTiles(spriteBatch, tileRect);

            var itemsToDraw = GlobalServices.GameState.AllItemsInRectangle(tileRect);
            var drawQueue = new PriorityQueue<int, StaticItem>(100);
            foreach (var item in itemsToDraw)
                {
                drawQueue.Enqueue(item.DrawOrder, item);
                }
            //Trace.WriteLine("Drawing " + drawQueue.Count + " sprites.");
            while (drawQueue.Count > 0)
                {
                var item = drawQueue.Dequeue();
                item.Draw(gameTime, spriteBatch);
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

                    Texture2D texture = this._tiles[x, y].Floor;
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
                var startPos = new TilePos(newState.Position.X + 2, newState.Position.Y);
                if (GetFreeTileWithinRoom(startPos, out var boulderPosition))
                    {
                    boulder.ResetPosition(boulderPosition.ToPosition());
                    }
                }
            Point roomStart = GetContainingRoom(this.Player.Position).Location;
            this.WindowPosition = new Vector2(roomStart.X, roomStart.Y);
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
        }
    }
