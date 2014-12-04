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
        private readonly List<int> _listOfWorldAreaIdsVisited = new List<int>();

        // Constants for controling horizontal movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 2;

        private int _countOfShotsBeforeCostingEnergy;
        private double _time;
        private int _countBeforeDecrementingEnergy;
        private const double TicksOfClock = 0.05f;

        private readonly IGameSoundInstance _playerMovesFootOne;
        private readonly IGameSoundInstance _playerMovesFootTwo;
        private bool _whichFootFlag;

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(World world, Vector2 position, int energy) : base(world, position)
            {
            // Load animated textures.
            _leftRightAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerLeftFacing", 1);
            _upAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerUpFacing", 1);
            _downAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerDownFacing", 1);

            Ap.NewFrame += PlayerSpriteNewFrame;
            
            Reset(position, energy);

            this._playerMovesFootOne = world.Game.SoundLibrary.GetSoundEffectInstance(GameSound.PlayerMoves);
            this._playerMovesFootTwo = world.Game.SoundLibrary.GetSoundEffectInstance(GameSound.PlayerMoves);
            this._playerMovesFootTwo.Pitch = -0.15f;

            this.ObjectCapability = ObjectCapability.CanPushOrCauseBounceBack;
            }

        private void PlayerSpriteNewFrame(object sender, EventArgs e)
            {
            var playerMoves = _whichFootFlag ? this._playerMovesFootOne : this._playerMovesFootTwo;
            playerMoves.Play();
            _whichFootFlag = !_whichFootFlag;
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

            CheckIfEnteredNewWorldArea();
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
        public override bool Update(GameTime gameTime)
            {
            bool result = ApplyInput(gameTime);

            if (!IsExtant)
                return false;
            
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

            return result;
            }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        private bool ApplyInput(GameTime gameTime)
            {
            bool changedFacingDirection = false;
            
            bool isFiring;
            bool moveToNextLevel;
            bool toggleFullScreen;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Direction requestedDirection = Input.GetRequestedDirectionOfMovement(out isFiring, out moveToNextLevel, out toggleFullScreen);

            if (moveToNextLevel && this.Direction == Direction.None)
                this.World.MoveUpALevel();
            if (toggleFullScreen)
                this.World.Game.ToggleFullScreen();

            // deal with changing which direction the player faces
            if (requestedDirection != Direction.None && requestedDirection != this._currentDirectionFaced)
                {
                this._currentDirectionFaced = requestedDirection;
            
                changedFacingDirection = true;
                _lastChangeOfDirection = gameTime.TotalGameTime;
                }

            bool isMoving = this.Direction != Direction.None;
            if (isMoving)
                {  // currently in motion
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
                isMoving = this.Direction != Direction.None;
                }

            if (isFiring)
                {
                TilePos startPos = TilePos.GetPositionAfterOneMove(this.TilePosition, this._currentDirectionFaced);
                if (this.World.CanTileBeOccupied(startPos, false) && this.Energy >= 4)
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
            
            if (isMoving)
                {
                // still in motion
                if (CheckIfEnteredNewWorldArea())
                    this.World.Game.SoundLibrary.Play(GameSound.PlayerEntersNewLevel);

                return true;
                }

            if (requestedDirection == Direction.None)
                // no requested movement
                return false;
            
            if (changedFacingDirection)
                return false;
            
            TimeSpan timeSinceChangedDirection = gameTime.TotalGameTime - this._lastChangeOfDirection;
            if (timeSinceChangedDirection.Milliseconds < 75)
                return false;
            
            // start new movement
            
            TilePos tp = this.TilePosition;
            TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(tp, requestedDirection);
            Vector2 potentiallyMovingTowards = potentiallyMovingTowardsTile.ToPosition();

            if (this.CanMoveTo(requestedDirection))
                {
                Direction = requestedDirection;
                this.MovingTowards = potentiallyMovingTowards;
                this.CurrentVelocity = StandardSpeed;
                }

            return false;
            }

        private bool CheckIfEnteredNewWorldArea()
            {
            int worldAreaId = this.World.GetWorldAreaIdForTilePos(this.TilePosition);
            if (this._listOfWorldAreaIdsVisited.Contains(worldAreaId))
                return false;

            this._listOfWorldAreaIdsVisited.Add(worldAreaId);
            return true;
            }

        // todo public void BounceBack(Direction bounceBackDirection, GameTime gameTime)
        public override void BounceBack(Direction direction)
            {
            if (this.CanMoveTo(direction))
                {
                this.Direction = direction;
                this.MovingTowards = this.TilePosition.GetPositionAfterOneMove(direction).ToPosition();
                this.CurrentVelocity = AnimationPlayer.BounceBackSpeed;
                // todo _lastChangeOfDirection = gameTime.TotalGameTime;
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

        public override void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException("energyToRemove", energyToRemove, "Must be above 0.");
            
            this.Energy -= energyToRemove;
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
            this.World.AddGrave(this.TilePosition);
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
        }
    }
