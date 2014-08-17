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

        private readonly LinkedList<StaticItem> _gameItems; 
        //private readonly LinkedList<StaticItem> _staticItems;
        //private readonly LinkedList<Shot> _shots;
        //private readonly LinkedList<Monster.Monster> _monsters;
        
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

            this._tiles = x.LoadLayout();

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

            this._gameItems = new LinkedList<StaticItem>();

            this.Player = new Player(this, ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            this._gameItems.AddLast(this.Player);
            Reset(spw);

            this.Boulder = new Boulder(this, ss.BlockPosition.ToPosition());
            this._gameItems.AddLast(this.Boulder);

            var monsters = x.GetListOfMonsters(this);
            //var toRemove = monsters.Where(m => !(m is RotaFloaterBrown) || m.Energy != 40 || m.Mobility != MonsterMobility.Patrolling).ToList();
            //foreach (var m in toRemove)
            //    monsters.Remove(m);
            this._gameItems.AddRange(monsters);

            CheckOccupationByStaticMonsters(exceptions);
            
            var crystals = x.GetListOfCrystals(this).ToList();
            this._gameItems.AddRange(crystals);
            AddCrystalsToOccupiedList(crystals, exceptions);

            var forceFields = x.GetListOfForceFields(this).ToList();
            this._gameItems.AddRange(forceFields);
            AddForceFieldsToOccupiedList(forceFields, exceptions);
            
            var fruit = x.GetListOfFruit(this, this._tiles);
            this._gameItems.AddRange(fruit);

            CheckOccupationByMovingMonsters(exceptions);
            
            //this._shots = new LinkedList<Shot>();
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

            this._areasVisited = new Stack<WorldArea>();
            this._areasVisited.Push(x.GetWorldAreaForTilePos(ss.PlayerPosition));
            }
        
        private void CheckOccupationByStaticMonsters(List<TileException> exceptions)
            {
            foreach (Monster.Monster m in this._gameItems.OfType<Monster.Monster>().Where(m => m.IsStill))
                {
                TilePos tp = TilePos.TilePosFromPosition(m.Position);

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
            foreach (Monster.Monster m in this._gameItems.OfType<Monster.Monster>().Where(m => !m.IsStill))
                {
                TilePos tp = TilePos.TilePosFromPosition(m.Position);

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
                TilePos tp = TilePos.TilePosFromPosition(c.Position);
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
                TilePos tp = TilePos.TilePosFromPosition(ff.Position);
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
            var item = this._gameItems.First;
            while (item != null)
                {
                var currentNode = item;
                item = item.Next;

                if (currentNode.Value is Bang || currentNode.Value is Shot)
                    this._gameItems.Remove(currentNode);
                }
            //this._shots.Clear();

            StartState ss = _wl.StartStateAfterDeath(TilePos.TilePosFromPosition(this.Player.Position));
            this.Player.Reset(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            Reset(spw);
            }
        
        private void Reset(SpriteBatchWindowed spw)
            {
            Point roomStart = GetContainingRoom(Player.Position).Location;
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
        private TileCollision GetCollision(int x, int y)
            {
            // Prevent escaping past the World ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the World top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Impassable;

            TileCollision result = _tiles[x, y].Collision;
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
        
        public bool CanPlayerMove(Direction d, Vector2 potentiallyMovingTowards)
            {
            if (d == Direction.None)
                throw new ArgumentOutOfRangeException("d", "Invalid direction");
            
            bool result;
            if (potentiallyMovingTowards == Boulder.Position && Boulder.Direction == Direction.None)
                {
                PushStatus ps = this.Boulder.CanBePushed(d);
                result = (ps != PushStatus.No);
                }
            else
                {
                result = IsTileUnoccupied(potentiallyMovingTowards, false);
                }
            return result;
            }
        
        public bool IsTileUnoccupied(TilePos tp, bool includeBlock)
            {
            TileCollision tc = GetCollision(tp.X, tp.Y);
            bool result = (tc != TileCollision.Impassable);
            if (result && includeBlock)
                {
                TilePos bp = TilePos.TilePosFromPosition(this.Boulder.Position);
                if (tp == bp)
                    result = false;
                }
            return result;
            }

        public bool IsTileUnoccupied(Vector2 position, bool includeBlock)
            {
            TilePos tp = TilePos.TilePosFromPosition(position);
            return IsTileUnoccupied(tp, includeBlock);
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
            
            //if (Player.IsExtant)
            //    {
            //    UpdatePlayer(gameTime);
            //    }
            
            //UpdateBlock(gameTime);
            UpdateGameItems(gameTime);
            //UpdateShots(gameTime);
            //UpdateMonsters(gameTime);
            
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
                TilePos tp = TilePos.TilePosFromPosition(this.Player.Position);
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
            foreach (Monster.Monster m in this._gameItems.OfType<Monster.Monster>())
                {
                if (m.IsEgg || m.IsActive || !m.IsExtant || m.ChangeRooms == ChangeRooms.StaysWithinRoom)
                    continue;
                
                WorldArea wa = this._wl.GetWorldAreaForTilePos(TilePos.TilePosFromPosition(m.Position));
                if (wa.HasId && wa.Id < levelThatPlayerShouldHaveReached)
                    m.IsActive = true;
                }
            }

        private void UpdateGameItems(GameTime gameTime)
            {
            var item = this._gameItems.First;
            while (item != null)
                {
                var currentItem = item;
                item = item.Next;
                
                if (!currentItem.Value.IsExtant && !(currentItem.Value is Player))
                    {
                    this._gameItems.Remove(currentItem);
                    continue;
                    }

                var mi = currentItem.Value as MovingItem;
                if (mi == null)
                    continue;
                mi.Update(gameTime);

                var m = mi as Monster.Monster;
                if (m != null && !m.IsActive)
                    continue;
                var bounds = mi.BoundingRectangle;
                for (var si = currentItem.Next; si != null; si = si.Next)
                    {
                    if (bounds.Intersects(si.Value.BoundingRectangle))
                        {
                        BuildInteraction(mi, si.Value).Collide();
                        }
                    }
                }
            }

        private IInteraction BuildInteraction(MovingItem mi, StaticItem si)
            {
            IInteraction result;
            var items = new[] { mi, si };
            var shot = items.OfType<Shot>().FirstOrDefault();
            if (shot != null)
                {
                var otherItem = items.Single(item => item != shot);
                var mi2 = otherItem as MovingItem;
                result = mi2 != null
                    ? (IInteraction) new ShotAndMovingItemInteraction(this, shot, mi2)
                    : new ShotAndStaticItemInteraction(this, shot, otherItem);
                }
            else
                {
                var mi2 = si as MovingItem;
                result = mi2 != null
                    ? (IInteraction) new MovingItemAndMovingItemInteraction(this, mi, mi2)
                    : new MovingItemAndStaticItemInteraction(this, mi, si);
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
            this._gameItems.AddLast(dd);
            }

        /// <summary>
        /// Place short bang at a shot's position and remove the shot
        /// </summary>
        /// <param name="s">The</param>
        public void ConvertShotToBang(Shot s)
            {
            var b = new Bang(this, s.Position, BangType.Short);
            this._gameItems.AddLast(b);  
            s.InstantDeath();
            }
        
        public void AddShortBang(Vector2 p)
            {
            var b = new Bang(this, p, BangType.Short);
            this._gameItems.AddLast(b);
            }
            
        /// <summary>
        /// Add long bang (when player or monster dies)
        /// </summary>
        /// <param name="p">Position to place the sprite</param>
        public void AddLongBang(Vector2 p)
            {
            var b = new Bang(this, p, BangType.Long);
            this._gameItems.AddLast(b);  
            }

        public void AddGrave(Vector2 position)
            {
            TilePos tp = TilePos.TilePosFromPosition(position);
            var g = new Grave(this, tp.ToPosition());
            this._gameItems.AddLast(g);
            }
        
        public void AddMushroom(Vector2 position)
            {
            var m = new Mushroom(this, position);
            this._gameItems.AddLast(m);
            }
        
        public void AddMonster(Monster.Monster m)
            {
            this._gameItems.AddLast(m);
            }
        
        public void AddShot(ShotType st, Vector2 position, int energy, Direction d)
            {
            var s = new Shot(this, position, d, energy, st);
            this._gameItems.AddLast(s);
            }
        
        public static Rectangle GetContainingRoom(Vector2 position)
            {
            TilePos tp = TilePos.TilePosFromPosition(position);
            int roomx = tp.X / WindowSizeX;
            int roomy = tp.Y / WindowSizeY;
            var r = new Rectangle(roomx * WindowSizeX * Tile.Width, roomy * WindowSizeY * Tile.Height, WindowSizeX * Tile.Width, WindowSizeY * Tile.Height);
            return r;
            }
        
        private void RecalculateWindow(GameTime gameTime, SpriteBatchWindowed spriteBatchWindowed)
            {
            if (this.Player == null)
                return;
            
            Point movingTowards = GetContainingRoom(Player.Position).Location;
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
            
            DrawTiles(spriteBatch);

            foreach (StaticItem si in this._gameItems.Where(m => !(m is Bang)))
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
            }

        /// <summary>
        /// Draws each tile in the World.
        /// </summary>
        private void DrawTiles(SpriteBatchWindowed spriteBatch)
            {
            const int windowWidth = (WindowSizeX - 1) * Tile.Width;
            const int windowHeight = (WindowSizeY - 1) * Tile.Height;
            
            var roomStartX = (int)Math.Floor(spriteBatch.WindowOffset.X / Tile.Width);
            var roomStartY = (int)Math.Floor(spriteBatch.WindowOffset.Y / Tile.Height);
            var roomEndX = (int)Math.Ceiling((spriteBatch.WindowOffset.X + windowWidth)  / Tile.Width);
            var roomEndY = (int)Math.Ceiling((spriteBatch.WindowOffset.Y + windowHeight) / Tile.Height);
            
            // draw the floor
            for (int y = roomStartY; y <= roomEndY; y++)
                {
                for (int x = roomStartX; x <= roomEndX; x++)
                    {
                    Texture2D texture = _tiles[x, y].Floor;
                    if (texture != null)
                        {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.DrawInWindow(texture, position);
                        }
                    }
                }

            // For each tile position
            for (int y = roomStartY; y <= roomEndY; y++)
                {
                for (int x = roomStartX; x <= roomEndX; x++)
                    {
                    // If there is a visible tile in that position
                    Texture2D texture = _tiles[x, y].Wall;
                    if (texture != null)
                        {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.DrawInWindow(texture, position);
                        }
                    }
                }
            }

        public bool ShotExists()
            {
            bool result = this._gameItems.OfType<Shot>().Any(item => item.IsExtant);
            return result;
            }

        public int HowManyCrystalsRemain()
            {
            int result = this._gameItems.OfType<Crystal>().Count(c => c.IsExtant);
            return result;
            }

        public void MoveUpALevel()
            {
            if (!this.Player.IsAlive())
                return;

            var currentWorld = this._wl.GetWorldAreaForTilePos(TilePos.TilePosFromPosition(this.Player.Position)).Id;
            var maxId = this._wl.GetMaximumId();
            StartState newState = null;
            for (int i = currentWorld + 1; i <= maxId; i++)
            {
                newState = this._wl.GetStartStateForWorld(i);
                if (newState != null)
                    break;
            }
            var itemToReplace = this._gameItems.Find(this.Boulder);
            if (newState == null || itemToReplace == null)
                return;

            var crystals = this._gameItems.OfType<Crystal>().Where(c => this._wl.GetWorldAreaForTilePos(TilePos.TilePosFromPosition(c.Position)).Id == currentWorld);
            foreach (var c in crystals)
                {
                var i = new MovingItemAndStaticItemInteraction(this, this.Player, c);
                i.Collide();
                }
            this.Player.Reset(newState.PlayerPosition.ToPosition(), newState.PlayerEnergy);
            this.Boulder = new Boulder(this, newState.BlockPosition.ToPosition());
            itemToReplace.Value = this.Boulder;
            Point roomStart = GetContainingRoom(Player.Position).Location;
            this.Game.SpriteBatchWindowed.WindowOffset = new Vector2(roomStart.X, roomStart.Y);
            }
        }
    }
