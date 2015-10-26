using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Labyrinth.Annotations;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public class World : IDisposable
        {
        public const int WindowSizeX = 16;
        public const int WindowSizeY = 10;

        private int _gameClock;
        private double _time;
        private int _levelUnlocked;
        private LevelReturnType _levelReturnType = LevelReturnType.Normal;
        private bool _doNotUpdate;
        
        [NotNull] public readonly Game1 Game;
        [NotNull] public readonly Player Player;
        public ISpriteLibrary SpriteLibrary;

        [NotNull] private readonly IWorldLoader _wl;
        [NotNull] private readonly List<StaticItem>[] _itemsToDrawByZOrder;

        public World(Game1 game, IWorldLoader worldLoader)
            {
            this.Game = game;
            this._wl = worldLoader;
            this.SpriteLibrary = new SpriteLibrary();

            var gameItems = worldLoader.GetGameObjects(this).ToList();
            this.Player = gameItems.OfType<Player>().Single();

            this._itemsToDrawByZOrder = new List<StaticItem>[10];
            for (int i = 0; i < this._itemsToDrawByZOrder.GetLength(0); i++)
                this._itemsToDrawByZOrder[i] = new List<StaticItem>();
            }

        [Obsolete]
        public Texture2D LoadTexture(string textureName)
            {
            var result = this.SpriteLibrary.GetSprite(textureName);
            return result;
            }

        public void ResetLevelAfterLosingLife(ISpriteBatch spw)
            {
            GlobalServices.GameState.RemoveBangsAndShots();
            var worldAreaId = this._wl.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            StartState ss = this._wl.GetStartStateForWorldAreaId(worldAreaId);
            this.Player.Reset(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            GlobalServices.GameState.UpdatePosition(this.Player);
            ResetLevelForStartingNewLife(spw);
            }
        
        public void ResetLevelForStartingNewLife(ISpriteBatch spw)
            {
            if (spw != null)
                {
                Point roomStart = GetContainingRoom(this.Player.Position).Location;
                spw.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
                }
            GlobalServices.SoundPlayer.Play(GameSound.PlayerStartsNewLife);
            this._levelReturnType = LevelReturnType.Normal;
            _doNotUpdate = false;
            }
        
        /// <summary>
        /// Unloads the World content.
        /// </summary>
        public void Dispose()
            {
            if (this.SpriteLibrary != null)
                {
                this.SpriteLibrary.Dispose();
                this.SpriteLibrary = null;
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

        public int GetWorldAreaIdForTilePos(TilePos tp)
            {
            var result = this._wl.GetWorldAreaIdForTilePos(tp);
            return result;
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
                
                int worldAreaId = this._wl.GetWorldAreaIdForTilePos(m.TilePosition);
                if (worldAreaId < levelThatPlayerShouldHaveReached)
                    m.IsActive = true;
                }
            }

        private void UpdateGameItems(GameTime gameTime)
            {
            const float minimumDistance = Tile.Height * Tile.Height;

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
                var currentItemPosition = currentItem.Position;
                foreach (var si in GlobalServices.GameState.AllItemsInRectangle(currentItem.BoundingRectangle))
                    {
                    if (currentItem == si)
                        continue;   

                    if (Math.Abs(Vector2.DistanceSquared(currentItemPosition, si.Position)) <= minimumDistance)
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
                            if (interaction != null)
                                interaction.Collide();
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

        public static Rectangle GetContainingRoom(Vector2 position)
            {
            const int roomWidth = WindowSizeX * Tile.Width;
            const int roomHeight = WindowSizeY * Tile.Height;

            var roomx = (int) (position.X / roomWidth);
            var roomy = (int) (position.Y / roomHeight);
            var r = new Rectangle(roomx * roomWidth, roomy * roomHeight, roomWidth, roomHeight);
            return r;
            }
        
        private void RecalculateWindow(GameTime gameTime, ISpriteBatch spriteBatchWindowed)
            {
            const float currentVelocity = 750;
            
            var roomRectangle = GetContainingRoom(this.Player.Position);
            var movingTowards = new Vector2(roomRectangle.Left, roomRectangle.Top);
            Vector2 position = spriteBatchWindowed.WindowOffset;
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
            
            spriteBatchWindowed.WindowOffset = position;
            }

        /// <summary>
        /// Draw everything in the World from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
            {
            RecalculateWindow(gameTime, spriteBatch);
            
            var viewPort = GetRectangleEnclosingTilesThatAreCurrentlyInView(spriteBatch.WindowOffset);
            DrawFloorTiles(spriteBatch, viewPort);
            
            var r = new Rectangle(viewPort.Left * Tile.Width, viewPort.Top * Tile.Height, viewPort.Width * Tile.Width, viewPort.Height * Tile.Height);
            var itemsToDraw = GlobalServices.GameState.AllItemsInRectangle(r);
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

        private static Rectangle GetRectangleEnclosingTilesThatAreCurrentlyInView(Vector2 windowOffset)
            {
            var roomStartX = (int)Math.Floor(windowOffset.X / Tile.Width);
            var roomStartY = (int)Math.Floor(windowOffset.Y / Tile.Height);
            
            const int windowWidth = WindowSizeX * Tile.Width;
            const int windowHeight = WindowSizeY * Tile.Height;
            
            var roomEndX = (int)Math.Ceiling((windowOffset.X + windowWidth) / Tile.Width);
            var roomEndY = (int)Math.Ceiling((windowOffset.Y + windowHeight) / Tile.Height);

            var result = new Rectangle(roomStartX, roomStartY, roomEndX - roomStartX, roomEndY - roomStartY);
            Debug.Assert(result.Width == WindowSizeX || result.Width == (WindowSizeX + 1));
            Debug.Assert(result.Height == WindowSizeY || result.Height == (WindowSizeY + 1));
            return result;
            }

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        private void DrawFloorTiles(ISpriteBatch spriteBatch, Rectangle r)
            {
            // draw the floor
            for (int y = r.Top; y < r.Bottom; y++)
                {
                for (int x = r.Left; x < r.Right; x++)
                    {
                    var tp = new TilePos(x, y);
                    Texture2D texture = this._wl[tp].Floor;
                    if (texture != null)
                        {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.DrawEntireTexture(texture, position);
                        }
                    }
                }
            }

        public void MoveUpALevel()
            {
            if (!this.Player.IsAlive())
                return;

            int currentWorldAreaId = this._wl.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            int maxId = this._wl.GetMaximumWorldAreaId();
            var newState = Enumerable.Range(currentWorldAreaId + 1, maxId - currentWorldAreaId).Select(i => this._wl.GetStartStateForWorldAreaId(i)).FirstOrDefault(startState => startState != null);
            if (newState == null)
                return;

            var crystals = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Where(c => this._wl.GetWorldAreaIdForTilePos(c.TilePosition) == currentWorldAreaId);
            foreach (var c in crystals)
                {
                var i = new InteractionWithStaticItems(this, c, this.Player);
                i.Collide();
                }
            this.Player.Reset(newState.PlayerPosition.ToPosition(), newState.PlayerEnergy);
            GlobalServices.GameState.UpdatePosition(this.Player);
            var boulder = GlobalServices.GameState.DistinctItemsOfType<Boulder>().FirstOrDefault();
            if (boulder != null)
                {
                TilePos? tp = null;
                for (int i = 0; i < 16; i++)
                    {
                    int x = (i % 2 == 0) ? 7 - (i / 2) : 8 + ((i - 1) / 2);
                    var potentialPosition = new TilePos(x, newState.PlayerPosition.Y);
                    if (!GlobalServices.GameState.IsStaticItemOnTile(potentialPosition))
                        {
                        tp = potentialPosition;
                        break;
                        }
                    }
                if (tp.HasValue)
                    {
                    boulder.Reset(tp.Value.ToPosition());
                    GlobalServices.GameState.UpdatePosition(boulder);
                    }
                }
            Point roomStart = GetContainingRoom(this.Player.Position).Location;
            this.Game.SpriteBatch.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
            }
        }
    }
