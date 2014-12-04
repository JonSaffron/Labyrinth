using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Labyrinth.Monster;

namespace Labyrinth
    {
    class World : IDisposable
        {
        private int _gameClock;
        private double _time;
        private int _levelUnlocked;
        
        // Entities in the World.
        private readonly Player _player;
        private readonly LinkedList<StaticItem> _allGameItems; 
        private readonly LinkedList<StaticItem> _interactiveGameItems; 
        
        bool _doNotUpdate;
        private readonly IWorldLoader _wl;
        
        // World content.   
        public readonly Game1 Game;
        private ContentManager _contentManager;

        public const int WindowSizeX = 16;
        public const int WindowSizeY = 10;
            
        private LevelReturnType _levelReturnType = LevelReturnType.Normal;

        public World(Game1 game, IWorldLoader worldLoader)
            {
            this.Game = game;
            this._wl = worldLoader;
            // Create a new content manager to load content used just by this World.
            this._contentManager = new ContentManager(game.Services, "Content");

            var gameItems = worldLoader.GetGameObjects(this).ToList();
            this._allGameItems = new LinkedList<StaticItem>(gameItems);
            this._interactiveGameItems = new LinkedList<StaticItem>(gameItems.Where(item => !(item is Wall)));
            this._player = gameItems.OfType<Player>().Single();
            }

        public Player Player
            {
            get
                {
                return _player;
                }
            }

        public Texture2D LoadTexture(string textureName)
            {
            var result = this._wl.LoadTexture(this._contentManager, textureName);
            return result;
            }

        public void ResetLevel(SpriteBatchWindowed spw)
            {
            var item = this._interactiveGameItems.First;
            while (item != null)
                {
                var currentNode = item;
                item = item.Next;

                if (currentNode.Value is Bang || currentNode.Value is Shot)
                    {
                    this._allGameItems.Remove(currentNode.Value);
                    this._interactiveGameItems.Remove(currentNode);
                    }
                }

            var worldAreaId = this._wl.GetWorldAreaIdForTilePos(this.Player.TilePosition);
            StartState ss = this._wl.GetStartStateForWorldAreaId(worldAreaId);
            this.Player.Reset(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            Reset(spw);
            }
        
        public void Reset(SpriteBatchWindowed spw)
            {
            Point roomStart = GetContainingRoom(Player.TilePosition).Location;
            spw.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
            this.Game.SoundLibrary.Play(GameSound.PlayerStartsNewLife);
            this._levelReturnType = LevelReturnType.Normal;
            _doNotUpdate = false;
            }
        
        [PublicAPIAttribute]
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Unloads the World content.
        /// </summary>
        public void Dispose()
            {
            Dispose(true);
            }

        ~World()
            {
            Dispose(false);
            }

        [PublicAPIAttribute]
        protected void Dispose(bool isDisposing)
            {
            if (isDisposing)
                {
                if (this._contentManager != null)
                    {
                    this._contentManager.Unload();
                    this._contentManager.Dispose();
                    this._contentManager = null;
                    }
                }

            this.IsDisposed = true;
            }

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the World and fall off the bottom.
        /// </summary>
        private bool IsTileWithinWorld(TilePos tp)
            {
            var x = tp.X;
            var y = tp.Y;
            var result = !(x < 0 || x >= this._wl.Width || y < 0 || y >= this._wl.Height);
            return result;
            }

        public void IncreaseScore(int score)
            {
            this.Game.AddScore(score);
            }
        
        public bool CanTileBeOccupied(TilePos tp, bool treatMoveableItemsAsImpassable)
            {
            if (!IsTileWithinWorld(tp))
                return false;

            var objectsAtPosition = GetItemsOnTile(tp);
            var isTileAlreadyOccupied = treatMoveableItemsAsImpassable
                ? objectsAtPosition.Any(gi => gi.Solidity == ObjectSolidity.Impassable || gi.Solidity == ObjectSolidity.Moveable)
                : objectsAtPosition.Any(gi => gi.Solidity == ObjectSolidity.Impassable);
            var result = !isTileAlreadyOccupied;
            return result;
            }

        public bool IsStaticItemOnTile(TilePos tp)
            {
            var objectsAtPosition = GetItemsOnTile(tp);
            var result = objectsAtPosition.Any(gi => !(gi is MovingItem));
            return result;
            }

        public IEnumerable<StaticItem> GetItemsOnTile(TilePos tp)
            {
            var result = this._allGameItems.Where(gi => gi.IsExtant && gi.TilePosition == tp);
            return result;
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
            _time += gameTime.ElapsedGameTime.TotalSeconds;
            while (_time > AnimationPlayer.GameClockResolution)
                {
                _time -= AnimationPlayer.GameClockResolution;
                if (this._gameClock < int.MaxValue)
                    this._gameClock++; 

                int levelToUnlock = this._gameClock >> 13;
                while (this._levelUnlocked < levelToUnlock)
                    {
                    this._levelUnlocked++;
                    UnlockLevel(this._levelUnlocked);
                    }
                }

            if (Player.Direction != Direction.None)
                {
                }
            }

        private void UnlockLevel(int levelThatPlayerShouldHaveReached)
            {
            foreach (Monster.Monster m in this._interactiveGameItems.OfType<Monster.Monster>())
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

            var item = this._interactiveGameItems.First;

            int i = 0;
            int j = 0;
            while (item != null)
                {
                var currentItem = item.Value;
                item = item.Next;
                j++;

                if (!currentItem.IsExtant && !(currentItem is Player))
                    {
                    this._interactiveGameItems.Remove(currentItem);
                    this._allGameItems.Remove(currentItem);
                    continue;
                    }

                var mi = currentItem as MovingItem;
                bool hasMoved = mi != null && mi.Update(gameTime);
                if (!hasMoved)
                    continue;

                Rectangle? bounds = null;
                var currentItemPosition = currentItem.Position;
                for (var si = this._interactiveGameItems.First; si != null; si = si.Next)
                    {
                    if (currentItem == si.Value)
                        continue;   

                    i++;

                    if (Math.Abs(Vector2.DistanceSquared(currentItemPosition, si.Value.Position)) <= minimumDistance)
                        {
                        if (bounds == null)
                            {
                            System.Diagnostics.Trace.WriteLine(string.Format("checking {0} and {1}", currentItem, si.Value));
                            bounds = currentItem.BoundingRectangle;
                            }
                        if (bounds.Value.Intersects(si.Value.BoundingRectangle))
                            {
                            System.Diagnostics.Trace.WriteLine(string.Format("interacting {0} and {1}", currentItem, si.Value));
                            BuildInteraction(currentItem, si.Value).Collide();
                            }
                        }
                    }
                }
                System.Diagnostics.Trace.WriteLine(string.Format("Checks run: {0}, from {1} items", i, j));
            }

        private IInteraction BuildInteraction(StaticItem thisGameItem, StaticItem thatGameItem)
            {
            IInteraction result;
            var items = new[] { thisGameItem, thatGameItem };
            var shot = items.OfType<Shot>().FirstOrDefault();
            if (shot != null)
                {
                var otherItem = items.Single(item => item != shot);
                var movingItem = otherItem as MovingItem;
                result = movingItem != null
                    ? (IInteraction) new ShotAndMovingItemInteraction(this, shot, movingItem)
                    : new ShotAndStaticItemInteraction(this, shot, otherItem);
                }
            else
                {
                var movingItem = items.OfType<MovingItem>().FirstOrDefault();
                if (movingItem != null)
                    {
                    var otherItem = items.Single(item => item != movingItem);
                    var otherMovingItem = otherItem as MovingItem;
                    result = otherMovingItem != null
                        ? (IInteraction) new MovingItemAndMovingItemInteraction(this, movingItem, otherMovingItem)
                        : new MovingItemAndStaticItemInteraction(this, movingItem, otherItem);
                    }
                else
                    result = new StaticItemAndStaticItemInteraction(this, thisGameItem, thatGameItem);
                }
            return result;
            }

        public void AddDiamondDemon(Monster.Monster m)
            {
            var dd = new DiamondDemon(this, m.Position, 30)
                         {
                             Mobility = MonsterMobility.Aggressive,
                             LaysEggs = true,
                             ChangeRooms = ChangeRooms.FollowsPlayer,
                             MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately
                         };
            this._interactiveGameItems.AddLast(dd);
            this._allGameItems.AddLast(dd);
            }

        /// <summary>
        /// Place short bang at a shot's position and remove the shot
        /// </summary>
        /// <param name="s">The</param>
        public void ConvertShotToBang(Shot s)
            {
            var b = new Bang(this, s.Position, BangType.Short);
            this._interactiveGameItems.AddLast(b);  
            this._allGameItems.AddLast(b);  
            s.InstantDeath();
            }
        
        public void AddShortBang(Vector2 p)
            {
            var b = new Bang(this, p, BangType.Short);
            this._interactiveGameItems.AddLast(b);
            this._allGameItems.AddLast(b);
            }
            
        /// <summary>
        /// Add long bang (when player or monster dies)
        /// </summary>
        /// <param name="p">Position to place the sprite</param>
        public void AddLongBang(Vector2 p)
            {
            var b = new Bang(this, p, BangType.Long);
            this._interactiveGameItems.AddLast(b);  
            this._allGameItems.AddLast(b);  
            }

        public void AddGrave(TilePos tp)
            {
            var g = new Grave(this, tp.ToPosition());
            this._interactiveGameItems.AddLast(g);
            this._allGameItems.AddLast(g);
            }
        
        public void AddMushroom(TilePos tp)
            {
            var m = new Mushroom(this, tp.ToPosition());
            this._interactiveGameItems.AddLast(m);
            this._allGameItems.AddLast(m);
            }
        
        public void AddMonster(Monster.Monster m)
            {
            this._interactiveGameItems.AddLast(m);
            this._allGameItems.AddLast(m);
            }
        
        public void AddShot(ShotType st, Vector2 position, int energy, Direction d)
            {
            var s = new Shot(this, position, d, energy, st);
            this._interactiveGameItems.AddLast(s);
            this._allGameItems.AddLast(s);
            }
        
        public static Rectangle GetContainingRoom(TilePos tp)
            {
            int roomx = tp.X / WindowSizeX;
            int roomy = tp.Y / WindowSizeY;
            var r = new Rectangle(roomx * WindowSizeX * Tile.Width, roomy * WindowSizeY * Tile.Height, WindowSizeX * Tile.Width, WindowSizeY * Tile.Height);
            return r;
            }
        
        private void RecalculateWindow(GameTime gameTime, SpriteBatchWindowed spriteBatchWindowed)
            {
            if (this.Player == null)
                return;
            
            Point movingTowards = GetContainingRoom(Player.TilePosition).Location;
            Vector2 position = spriteBatchWindowed.WindowOffset;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const float currentVelocity = 750;

            switch (Math.Sign(movingTowards.X - spriteBatchWindowed.WindowOffset.X))
                {
                case -1:
                    position += new Vector2(-currentVelocity, 0) * elapsed;
                    if (position.X < movingTowards.X)
                        {
                        position = new Vector2(movingTowards.X, position.Y);
                        }
                    break;
                case 1:
                    position += new Vector2(currentVelocity, 0) * elapsed;
                    if (position.X > movingTowards.X)
                        {
                        position = new Vector2(movingTowards.X, position.Y);
                        }
                    break;
                }
            switch (Math.Sign(movingTowards.Y - spriteBatchWindowed.WindowOffset.Y))
                {
                case -1:
                    position += new Vector2(0, -currentVelocity) * elapsed;
                    if (position.Y < movingTowards.Y)
                        {
                        position = new Vector2(position.X, movingTowards.Y);
                        }
                    break;
                case 1:
                    position += new Vector2(0, currentVelocity) * elapsed;
                    if (position.Y > movingTowards.Y)
                        {
                        position = new Vector2(position.X, movingTowards.Y);
                        }
                    break;
                }
            
            spriteBatchWindowed.WindowOffset = position;
            }

        /// <summary>
        /// Draw everything in the World from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatchWindowed spriteBatch)
            {
            RecalculateWindow(gameTime, spriteBatch);
            
            var viewPort = GetViewPort(spriteBatch.WindowOffset);

            DrawFloorTiles(spriteBatch, viewPort);

            var r = new Rectangle((viewPort.Left - 1) * Tile.Width, (viewPort.Top -1) * Tile.Height, (viewPort.Right + 2) * Tile.Width, (viewPort.Bottom + 1) * Tile.Height);

            foreach (var item in this._allGameItems)
                if (r.Contains((int) item.Position.X, (int) item.Position.Y))
                    item.Draw(gameTime, spriteBatch);
/*                return;

            foreach (Wall w in this._gameItems.OfType<Wall>())
                w.Draw(gameTime, spriteBatch);

            foreach (StaticItem si in this._gameItems.Where(m => !(m is MovingItem) && !(m is Bang)))
                si.Draw(gameTime, spriteBatch);
            
            Player.Draw(gameTime, spriteBatch);

            foreach (Monster.Monster m in this._gameItems.OfType<Monster.Monster>().Where(m => m.IsStill))
                m.Draw(gameTime, spriteBatch);

            foreach (Monster.Monster m in this._gameItems.OfType<Monster.Monster>().Where(m => !m.IsStill))
                m.Draw(gameTime, spriteBatch);
            
            Boulder.Draw(gameTime, spriteBatch);
            
            foreach (Shot s in this._gameItems.OfType<Shot>())
                s.Draw(gameTime, spriteBatch);

            foreach (Bang b in this._gameItems.OfType<Bang>())
                b.Draw(gameTime, spriteBatch);
*/            }

        public int GetZOrder(StaticItem si)
            {
            if (si is Wall)
                return 0;
            if (!(si is MovingItem) && !(si is Bang))
                return 1;
            var monster = si as Monster.Monster;
            if (monster != null)
                {
                var result = monster.IsStill ? 2 : 3;
                return result;
                }
            if (si is Player)
                return 4;
            if (si is Boulder)
                return 5;
            if (si is Shot)
                return 6;
            if (si is Bang)
                return 7;

            throw new InvalidOperationException();
            }

        private static Rectangle GetViewPort(Vector2 windowOffset)
            {
            const int windowWidth = (WindowSizeX - 1) * Tile.Width;
            const int windowHeight = (WindowSizeY - 1) * Tile.Height;
            
            var roomStartX = (int)Math.Floor(windowOffset.X / Tile.Width);
            var roomStartY = (int)Math.Floor(windowOffset.Y / Tile.Height);
            var roomEndX = (int)Math.Ceiling((windowOffset.X + windowWidth)  / Tile.Width);
            var roomEndY = (int)Math.Ceiling((windowOffset.Y + windowHeight) / Tile.Height);

            var result = new Rectangle(roomStartX, roomStartY, roomEndX - roomStartX, roomEndY - roomStartY);
            return result;
            }

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        private void DrawFloorTiles(SpriteBatchWindowed spriteBatch, Rectangle r)
            {
            // draw the floor
            for (int y = r.Top; y <= r.Bottom; y++)
                {
                for (int x = r.Left; x <= r.Right; x++)
                    {
                    var tp = new TilePos(x, y);
                    Texture2D texture = this._wl[tp].Floor;
                    if (texture != null)
                        {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.DrawInWindow(texture, position);
                        }
                    }
                }
            }

        public bool ShotExists()
            {
            bool result = this._interactiveGameItems.OfType<Shot>().Any(item => item.IsExtant);
            return result;
            }

        public int HowManyCrystalsRemain()
            {
            int result = this._interactiveGameItems.OfType<Crystal>().Count(c => c.IsExtant);
            return result;
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

            var crystals = this._interactiveGameItems.OfType<Crystal>().Where(c => this._wl.GetWorldAreaIdForTilePos(c.TilePosition) == currentWorldAreaId);
            foreach (var c in crystals)
                {
                var i = new MovingItemAndStaticItemInteraction(this, this.Player, c);
                i.Collide();
                }
            this.Player.Reset(newState.PlayerPosition.ToPosition(), newState.PlayerEnergy);
            var boulder = this._interactiveGameItems.OfType<Boulder>().FirstOrDefault();
            if (boulder != null)
                {
                TilePos? tp = null;
                for (int i = 0; i < 16; i++)
                    {
                    int x = (i % 2 == 0) ? 7 - (i / 2) : 8 + ((i - 1) / 2);
                    var potentialPosition = new TilePos(x, newState.PlayerPosition.Y);
                    if (!IsStaticItemOnTile(potentialPosition))
                        {
                        tp = potentialPosition;
                        break;
                        }
                    }
                if (tp.HasValue)
                    boulder.Reset(tp.Value.ToPosition());
                }
            Point roomStart = GetContainingRoom(Player.TilePosition).Location;
            this.Game.SpriteBatchWindowed.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
            }
        }
    }
