using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

/* If at grid point: On new keypress
 *  If not facing the same way then turn to face the indicated direction return
 *  Start moving in the direction faced. Cache the direction.
 * If at grid point and key is still pressed
 *  Keep moving in cached direction
 * If moving:
 *  If key released and haven't reached the next stop point then keep moving in cached direction
 *  If key released and have reached the next stop point then stop moving
 *  If new key pressed change direction being faced. Cache the direction.
 */

namespace Labyrinth
    {
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player : MovingItem
        {
        // Movement
        //public Vector2 MovingTowards {get; private set;}
        private Direction _currentDirectionFaced; // can't be none
        private TimeSpan _lastChangeOfDirection;

        // Animations
        private readonly Animation _leftRightAnimation;
        private readonly Animation _upAnimation;
        private readonly Animation _downAnimation;

        private readonly List<int> _crystalsCollected = new List<int>();
        
        // Constants for controling horizontal movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 2;
        private const float BounceBackSpeed = AnimationPlayer.BaseSpeed * 3;

        private int _countOfShotsBeforeCostingEnergy;
        private double _time;
        private int _countBeforeDecrementingEnergy;
        private const double TicksOfClock = 0.05f;

        private readonly SoundEffectInstance _playerMovesFootOne;
        private readonly SoundEffectInstance _playerMovesFootTwo;
        private bool _whichFootFlag;

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(World world, Vector2 position, int energy) : base(world, position)
            {
            // Load animated textures.
            _leftRightAnimation = Animation.LoopingAnimation(World.Content.Load<Texture2D>("Sprites/Player/PlayerLeftFacing"), 1);
            _upAnimation = Animation.LoopingAnimation(World.Content.Load<Texture2D>("Sprites/Player/PlayerUpFacing"), 1);
            _downAnimation = Animation.LoopingAnimation(World.Content.Load<Texture2D>("Sprites/Player/PlayerDownFacing"), 1);

            Ap.NewFrame += PlayerSpriteNewFrame;
            
            Reset(position, energy);

            this._playerMovesFootOne = world.Game.SoundLibrary[GameSound.PlayerMoves].CreateInstance();
            this._playerMovesFootTwo = world.Game.SoundLibrary[GameSound.PlayerMoves].CreateInstance();
            this._playerMovesFootTwo.Pitch = -0.15f;
            }

        private void PlayerSpriteNewFrame(object sender, EventArgs e)
        {
            var playerMoves = _whichFootFlag ? this._playerMovesFootOne : this._playerMovesFootTwo;
            //playerMoves.Stop();
            playerMoves.Play();
            _whichFootFlag = !_whichFootFlag;
            //this.World.Game.SoundLibrary.Play(GameSound.PlayerMoves);
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        /// <param name="energy">Initial energy</param>
        public void Reset(Vector2 position, int energy)
            {
            Position = position;
            Energy = energy;
            _currentDirectionFaced = Direction.Left;
            Direction = Direction.None;
            Ap.PlayAnimation(_leftRightAnimation);
            this._countOfShotsBeforeCostingEnergy = 0;  // first shot will cost energy
            this._time = 0;
            this._countBeforeDecrementingEnergy = 0;
            }

        public override bool IsExtant
            {
            get
                {
                var result = this.Energy > 0 || this._countBeforeDecrementingEnergy > 0;
                return result;
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override void Update(GameTime gameTime)
            {
            ApplyInput(gameTime);

            if (!IsExtant)
                return;
            
            Animation a = null;
            switch (this._currentDirectionFaced)
                {
                case Direction.Left:
                case Direction.Right:
                    a = _leftRightAnimation;
                    break;
                case Direction.Up:
                    a = _upAnimation;
                    break;
                case Direction.Down:
                    a = _downAnimation;
                    break;
                }
            Ap.PlayAnimation(a);

            _time += gameTime.ElapsedGameTime.TotalSeconds;
            while (_time > TicksOfClock)
                {
                _time -= TicksOfClock;

                this._countBeforeDecrementingEnergy--; 
                if (this._countBeforeDecrementingEnergy < 0)
                    {
                    _countBeforeDecrementingEnergy = ((this.Energy >> 1) ^ 0xFF) & 0x7F;
                    if (this.Energy > 0)
                        ReduceEnergy(1);
                    }
                }

            }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        private void ApplyInput(GameTime gameTime)
            {
            bool changedFacingDirection = false;
            
            bool isFiring;
            bool moveToNextLevel;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Direction d = Input.GetRequestedDirectionOfMovement(out isFiring, out moveToNextLevel);

            if (moveToNextLevel)
                this.World.MoveUpALevel();

            if (d != Direction.None && d != this._currentDirectionFaced)
                {
                this._currentDirectionFaced = d;
            
                changedFacingDirection = true;
                _lastChangeOfDirection = gameTime.TotalGameTime;
                }

            if (this.Direction != Direction.None)
                {   // currently in motion
                switch (this.Direction)
                    {
                    case Direction.Left:
                        Position += new Vector2(-CurrentVelocity, 0) * elapsed;
                        if (Position.X < MovingTowards.X)
                            {
                            Position = new Vector2(MovingTowards.X, Position.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Right:
                        Position += new Vector2(CurrentVelocity, 0) * elapsed;
                        if (Position.X > MovingTowards.X)
                            {
                            Position = new Vector2(MovingTowards.X, Position.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Up:
                        Position += new Vector2(0, -CurrentVelocity) * elapsed;
                        if (Position.Y < MovingTowards.Y)
                            {
                            Position = new Vector2(Position.X, MovingTowards.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Down:
                        Position += new Vector2(0, CurrentVelocity) * elapsed;
                        if (Position.Y > MovingTowards.Y)
                            {
                            Position = new Vector2(Position.X, MovingTowards.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    }
                }

            if (isFiring)
                {
                TilePos startPos = TilePos.GetPositionAfterOneMove(TilePos.TilePosFromPosition(this.Position), this._currentDirectionFaced);
                if (this.World.IsTileUnoccupied(startPos, false) && this.Energy >= 4)
                    {
                    this.World.AddShot(ShotType.Player, startPos.ToPosition(), this.Energy >> 2, this._currentDirectionFaced);
                    this.World.Game.SoundLibrary.Play(GameSound.PlayerShoots);
                    _countOfShotsBeforeCostingEnergy--;
                    if (_countOfShotsBeforeCostingEnergy < 0)
                        {
                        _countOfShotsBeforeCostingEnergy = 3;
                        ReduceEnergy(1);
                        }
                    }
                }
            
            if (this.Direction != Direction.None)
                // still in motion
                return;

            if (d == Direction.None)
                // no requested movement
                return;
            
            if (changedFacingDirection)
                return;
            
            TimeSpan timeSinceChangedDirection = gameTime.TotalGameTime - this._lastChangeOfDirection;
            if (timeSinceChangedDirection.Milliseconds < 75)
                return;
            
            // start new movement
            
            TilePos tp = TilePos.TilePosFromPosition(Position);
            TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(tp, d);
            Vector2 potentiallyMovingTowards = potentiallyMovingTowardsTile.ToPosition();

            if (World.CanPlayerMove(d, potentiallyMovingTowards))
                {
                Direction = d;
                this.MovingTowards = potentiallyMovingTowards;
                this.CurrentVelocity = StandardSpeed;
                }
            }

        // todo public void BounceBack(Direction bounceBackDirection, GameTime gameTime)
        public void BounceBack(Direction bounceBackDirection)
            {
            TilePos tp = TilePos.TilePosFromPosition(this.MovingTowards);
            TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(TilePos.GetPositionAfterOneMove(tp, bounceBackDirection), bounceBackDirection);
            
            if (World.IsTileUnoccupied(potentiallyMovingTowardsTile, false))
                {
                this.Direction = bounceBackDirection;
                this.MovingTowards = potentiallyMovingTowardsTile.ToPosition();
                this.CurrentVelocity = BounceBackSpeed;
                // todo _lastChangeOfDirection = gameTime.TotalGameTime;
                }
            }

        public bool IsBouncingBack
            {
            get
                {
                var result = this.Direction != Direction.None && Math.Abs(this.CurrentVelocity - BounceBackSpeed) < 1.0;
                return result;
                }
            }

        public void AddEnergy(int energy)
            {
            if (energy <= 0)
                throw new ArgumentOutOfRangeException("energy", energy, "Must be above 0.");
            this.Energy += energy;
            if (this.Energy > 255)
                this.Energy = 255;
            }

        public override void ReduceEnergy(int energy)
            {
            if (energy <= 0)
                throw new ArgumentOutOfRangeException("energy", energy, "Must be above 0.");
            
            this.Energy -= energy;
            if (this.Energy <= 0)
                {
                UponDeath();
                }
            }
        
        public void InstantDeath()
            {
            if (!this.IsExtant)
                return;

            UponDeath();
            }

        private void UponDeath()
            {
            this.Energy = 0;
            this._countBeforeDecrementingEnergy = 0;

            this.World.Game.SoundLibrary.Play(GameSound.PlayerDies, SoundEffectFinished);
            this.World.AddLongBang(this.Position);
            this.World.AddGrave(this.Position);
            }

        private void SoundEffectFinished(object sender, SoundEffectFinishedEventArgs args)
            {
            this.World.SetLevelReturnType(LevelReturnType.LostLife);
            }

        public void CrystalCollected(Crystal c)
            {
            this._crystalsCollected.Add(c.CrystalId);
            }
        
        public bool HasPlayerGotCrystal(int id)
            {
            bool result = this._crystalsCollected.Contains(id);
            return result;
            }
        
        /// <summary>
        /// Draws the animated player.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatchWindowed spriteBatch)
            {
            if (!IsExtant)
                return;
            
            // Flip the sprite to face the way we are moving.
            Ap.SpriteEffect = _currentDirectionFaced == Direction.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            bool isMoving = this.Direction != Direction.None && this.Direction == this._currentDirectionFaced;

            // Draw that sprite.
            Ap.Draw(gameTime, spriteBatch, Position, isMoving);
            }

        public override ShotStatus OnShot(Shot s)
            {
            if (!this.IsExtant)
                return ShotStatus.CarryOn;

            this.ReduceEnergy(this.Energy);
            this.World.Game.SoundLibrary.Play(GameSound.PlayerInjured);
            return ShotStatus.HitHome;
            }

        public override TouchResult OnTouched(Player p)
            {
            throw new InvalidOperationException();
            }
        }
    }
