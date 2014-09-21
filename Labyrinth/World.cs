using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Labyrinth.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Labyrinth.Monster;

namespace Labyrinth
    {
    /// <summary>
    /// A uniform grid of tiles with collections of gems and enemies.
    /// The World owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class World : IDisposable
        {
        private int _gameClock;
        private double _time;
        private int _levelUnlocked;
        
        // Physical structure of the World.
        private readonly Tile[,] _tiles;
        
        // Entities in the World.
        public Player Player { get; private set; }

        private Boulder Boulder { get; set; }

        private readonly LinkedList<StaticItem> _allGameItems; 
        private readonly LinkedList<StaticItem> _interactiveGameItems; 
        
        private readonly Stack<WorldArea> _areasVisited;

        bool _doNotUpdate;
        private readonly WorldLoader _wl;
        
        // World content.   
        public readonly Game1 Game;
        public ContentManager Content { get; private set; }

        public const int WindowSizeX = 16;
        public const int WindowSizeY = 10;
            
        private LevelReturnType _levelReturnType = LevelReturnType.Normal;

        public World(IServiceProvider serviceProvider, WorldLoader x, SpriteBatchWindowed spw, Game1 game)
            {
            this._wl = x;
            // Create a new content manager to load content used just by this World.
            Content = new ContentManager(serviceProvider, "Content");
            this.Game = game;

            var exceptions = new List<TileException>();

            var walls = new List<StaticItem>();
            this._tiles = x.LoadLayout(walls, this);

            StartState ss = x.StartStateForWorld;
            try
                {
                this._tiles[ss.PlayerPosition.X, ss.PlayerPosition.Y].SetOccupationByStaticItem();
                }
            catch (TileException te)
                {
                var nte = new TileException(te, string.Format("Player's start position at tile {0},{1}", ss.PlayerPosition.X, ss.PlayerPosition.Y));
                exceptions.Add(nte);
                }
            try
                {
                this._tiles[ss.BlockPosition.X, ss.BlockPosition.Y].SetOccupationByStaticItem();
                }
            catch (TileException te)
                {
                var nte = new TileException(te, string.Format("Boulder's start position at tile {0},{1}", ss.BlockPosition.X, ss.BlockPosition.Y));
                exceptions.Add(nte);
                }

            this._allGameItems = new LinkedList<StaticItem>();
            this._allGameItems.AddRange(walls);

            this.Player = new Player(this, ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            this._allGameItems.AddLast(this.Player);
            Reset(spw);

            this.Boulder = new Boulder(this, ss.BlockPosition.ToPosition());
            this._allGameItems.AddLast(this.Boulder);

            var monsters = x.GetListOfMonsters(this);
            //var toRemove = monsters.Where(m => !(m is RotaFloaterBrown) || m.Energy != 40 || m.Mobility != MonsterMobility.Patrolling).ToList();
            //foreach (var m in toRemove)
            //    monsters.Remove(m);
            this._allGameItems.AddRange(monsters);

            CheckOccupationByStaticMonsters(exceptions);
            
            var crystals = x.GetListOfCrystals(this).ToList();
            this._allGameItems.AddRange(crystals);
            AddCrystalsToOccupiedList(crystals, exceptions);

            var forceFields = x.GetListOfForceFields(this).ToList();
            this._allGameItems.AddRange(forceFields);
            AddForceFieldsToOccupiedList(forceFields, exceptions);
            
            var fruit = x.GetListOfFruit(this, this._tiles);
            this._allGameItems.AddRange(fruit);

            CheckOccupationByMovingMonsters(exceptions);
            
            ReviewPotentiallyOccupiedTiles(exceptions);
            
            if (exceptions.Count != 0)
                {
                string message = string.Empty;
                var errors = exceptions.Where(te=>te.Type == TileExceptionType.Error).ToList();
                var warnings = exceptions.Where(te=>te.Type == TileExceptionType.Warning).ToList();
                foreach (TileException te in errors)
                    {
                    if (message.Length != 0)
                        message += "\r\n";
                    message += string.Format("{0} - {1}", te.Type, te.Message);
                    }
                if (warnings.Any())
                    {
                    if (message.Length != 0)
                        message += "\r\n";
                    foreach (TileException te in warnings)
                        {
                        if (message.Length != 0)
                            message += "\r\n";
                        message += string.Format("{0} - {1}", te.Type, te.Message);
                        }
                    }
                if (errors.Any())
                    throw new InvalidOperationException(message);
                System.Diagnostics.Trace.WriteLine(message);
                var dr = MessageBox.Show(message, "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    throw new InvalidOperationException(message);
                }

            this._interactiveGameItems = new LinkedList<StaticItem>(this._allGameItems.Where(item => !(item is Wall)));
            this._areasVisited = new Stack<WorldArea>();
            this._areasVisited.Push(x.GetWorldAreaForTilePos(ss.PlayerPosition));
            }
        
        private void CheckOccupationByStaticMonsters(List<TileException> exceptions)
            {
            foreach (Monster.Monster m in this._allGameItems.OfType<Monster.Monster>().Where(m => m.IsStill))
                {
                TilePos tp = m.TilePosition;

                try
                    {
                    this._tiles[tp.X, tp.Y].SetOccupationByStaticItem();
                    }
                catch (TileException te)
                    {
                    var nte = new TileException(te, string.Format("Static monster {0} at tile {1},{2}", m.GetType().Name, tp.X, tp.Y));
                    exceptions.Add(nte);
                    }
                }
            }
        
        private void CheckOccupationByMovingMonsters(List<TileException> exceptions)
            {
            foreach (Monster.Monster m in this._allGameItems.OfType<Monster.Monster>().Where(m => !m.IsStill))
                {
                TilePos tp = m.TilePosition;

                try
                    {
                    this._tiles[tp.X, tp.Y].SetOccupationByMovingMonster();
                    }
                catch (TileException te)
                    {
                    var nte = new TileException(te, string.Format("Moving monster {0} at tile {1},{2}", m.GetType().Name, tp.X, tp.Y));
                    exceptions.Add(nte);
                    }
                }
            }

        private void AddCrystalsToOccupiedList(IEnumerable<Crystal> crystals, List<TileException> exceptions)
            {
            foreach (Crystal c in crystals)
                {
                TilePos tp = c.TilePosition;
                try
                    {
                    this._tiles[tp.X, tp.Y].SetOccupationByStaticItem();
                    }
                catch (TileException te)
                    {
                    var nte = new TileException(te, string.Format("Crystal at tile {0},{1}", tp.X, tp.Y));
                    exceptions.Add(nte);
                    }
                }
            }

        private void AddForceFieldsToOccupiedList(IEnumerable<ForceField> forceFields, List<TileException> exceptions)
            {
            foreach (ForceField ff in forceFields)
                {
                TilePos tp = ff.TilePosition;
                try
                    {
                    this._tiles[tp.X, tp.Y].SetOccupationByStaticItem();
                    }
                catch (TileException te)
                    {
                    var nte = new TileException(te, string.Format("ForceField at tile {0},{1}", tp.X, tp.Y));
                    exceptions.Add(nte);
                    }
                }
            }

        private void ReviewPotentiallyOccupiedTiles(List<TileException> exceptions)
            {
            for (int y = 0; y < _tiles.GetLength(1); y++)
                for (int x = 0; x < _tiles.GetLength(0); x++)
                    {
//                    if (this._wl.GetWorldAreaForTilePos(new TilePos(x, y)).Id > 5)
//                        continue;

                    Tile t = this._tiles[x, y];
                    try
                        {
                        t.CheckIfIncorrectlyPotentiallyOccupied();
                        }
                    catch (TileException te)
                        {
                        var nte = new TileException(te, string.Format("Tile at {0},{1}", x, y));
                        exceptions.Add(nte);
                        }
                    }
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
            //this._shots.Clear();

            StartState ss = _wl.StartStateAfterDeath(this.Player.TilePosition);
            this.Player.Reset(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            Reset(spw);
            }
        
        private void Reset(SpriteBatchWindowed spw)
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
                if (this.Content != null)
                    {
                    this.Content.Unload();
                    this.Content.Dispose();
                    this.Content = null;
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
            var result = !(x < 0 || x >= Width || y < 0 || y >= Height);
            return result;
            }

        /// <summary>
        /// Width of World measured in tiles.
        /// </summary>
        private int Width
            {
            get { return _tiles.GetLength(0); }
            }

        /// <summary>
        /// Height of the World measured in tiles.
        /// </summary>
        private int Height
            {
            get { return _tiles.GetLength(1); }
            }
        
        public void IncreaseScore(int score)
            {
            this.Game.AddScore(score);
            }
        
        //public bool CanPlayerMove(Direction d, Vector2 potentiallyMovingTowards)
        //    {
        //    if (d == Direction.None)
        //        throw new ArgumentOutOfRangeException("d", "Invalid direction");
            
        //    bool result;
        //    if (potentiallyMovingTowards == Boulder.Position && Boulder.Direction == Direction.None)
        //        {
        //        PushStatus ps = this.Boulder.CanBePushed(d);
        //        result = (ps != PushStatus.No);
        //        }
        //    else
        //        {
        //        result = CanTileBeOccupied(potentiallyMovingTowards, false);
        //        }
        //    return result;
        //    }
        
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
                TilePos tp = this.Player.TilePosition;
                WorldArea wa = this._wl.GetWorldAreaForTilePos(tp);
                if (!this._areasVisited.Contains(wa))
                    {
                    this._areasVisited.Push(wa);
                    this.Game.SoundLibrary.Play(GameSound.PlayerEntersNewLevel);
                    }
                }
            }

        private void UnlockLevel(int levelThatPlayerShouldHaveReached)
            {
            foreach (Monster.Monster m in this._interactiveGameItems.OfType<Monster.Monster>())
                {
                if (m.IsEgg || m.IsActive || !m.IsExtant || m.ChangeRooms == ChangeRooms.StaysWithinRoom)
                    continue;
                
                WorldArea wa = this._wl.GetWorldAreaForTilePos(m.TilePosition);
                if (wa.HasId && wa.Id < levelThatPlayerShouldHaveReached)
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

            DrawTiles(spriteBatch, viewPort);

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

        private Rectangle GetViewPort(Vector2 windowOffset)
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
        /// Draws each tile in the World.
        /// </summary>
        private void DrawTiles(SpriteBatchWindowed spriteBatch, Rectangle r)
            {
            // draw the floor
            for (int y = r.Top; y <= r.Bottom; y++)
                {
                for (int x = r.Left; x <= r.Right; x++)
                    {
                    Texture2D texture = _tiles[x, y].Floor;
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

            var currentWorld = this._wl.GetWorldAreaForTilePos(this.Player.TilePosition).Id;
            var maxId = this._wl.GetMaximumId();
            StartState newState = null;
            for (int i = currentWorld + 1; i <= maxId; i++)
            {
                newState = this._wl.GetStartStateForWorld(i);
                if (newState != null)
                    break;
            }
            var itemToReplace = this._interactiveGameItems.Find(this.Boulder);
            if (newState == null || itemToReplace == null)
                return;

            var crystals = this._interactiveGameItems.OfType<Crystal>().Where(c => this._wl.GetWorldAreaForTilePos(c.TilePosition).Id == currentWorld);
            foreach (var c in crystals)
                {
                var i = new MovingItemAndStaticItemInteraction(this, this.Player, c);
                i.Collide();
                }
            this.Player.Reset(newState.PlayerPosition.ToPosition(), newState.PlayerEnergy);
            this.Boulder = new Boulder(this, newState.BlockPosition.ToPosition());
            itemToReplace.Value = this.Boulder;
            Point roomStart = GetContainingRoom(Player.TilePosition).Location;
            this.Game.SpriteBatchWindowed.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
            }
        }
    }
