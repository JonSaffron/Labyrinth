using System;
using System.Collections.Generic;
using Labyrinth.Annotations;
using Microsoft.Xna.Framework;
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

namespace Labyrinth.GameObjects
    {
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player : MovingItem
        {
        // Movement
        private Direction _currentDirectionFaced; // can't be none
        private TimeSpan _whenCanMoveInDirectionFaced;
        private readonly TimeSpan _delayBeforeMovingInDirectionFaced = TimeSpan.FromMilliseconds(75);
        
        [NotNull] private IPlayerWeapon _weapon1;
        [NotNull] private IPlayerWeapon _weapon2;

        // Animations
        private readonly Animation _leftRightMovingAnimation;
        private readonly Animation _upMovingAnimation;
        private readonly Animation _downMovingAnimation;
        private readonly Animation _leftRightStaticAnimation;
        private readonly Animation _upStaticAnimation;
        private readonly Animation _downStaticAnimation;

        private readonly List<int> _crystalsCollected = new List<int>();
        private readonly List<int> _listOfWorldAreaIdsVisited = new List<int>();

        private double _time;
        private int _countBeforeDecrementingEnergy;
        private const double TicksOfClock = 0.05f;

        private readonly IGameSoundInstance _playerMovesFootOne;
        private readonly IGameSoundInstance _playerMovesFootTwo;
        private bool _whichFootFlag;

        /// <summary>
        /// Constructs a new player.
        /// </summary>
        public Player(World world, Vector2 position, int energy) : base(world, position)
            {
            // Load animated textures.
            this._leftRightMovingAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerLeftFacing", 1);
            this._upMovingAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerUpFacing", 1);
            this._downMovingAnimation = Animation.LoopingAnimation(World, "Sprites/Player/PlayerDownFacing", 1);
            this._leftRightStaticAnimation = Animation.StaticAnimation(World, "Sprites/Player/PlayerLeftFacing");
            this._upStaticAnimation = Animation.StaticAnimation(World, "Sprites/Player/PlayerUpFacing");
            this._downStaticAnimation = Animation.StaticAnimation(World, "Sprites/Player/PlayerDownFacing");

            Ap.NewFrame += PlayerSpriteNewFrame;
            
            this._weapon1 = new StandardPlayerWeapon();
            this._weapon2 = new MineLayer();
            Reset(position, energy);

            this._playerMovesFootOne = world.Game.SoundPlayer.GetSoundEffectInstance(GameSound.PlayerMoves);
            this._playerMovesFootTwo = world.Game.SoundPlayer.GetSoundEffectInstance(GameSound.PlayerMoves);
            this._playerMovesFootTwo.Pitch = -0.15f;
            }

        private void PlayerSpriteNewFrame(object sender, EventArgs e)
            {
            var playerMoves = _whichFootFlag ? this._playerMovesFootOne : this._playerMovesFootTwo;
            playerMoves.Play(this.World.Game.SoundPlayer);
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
            this._currentDirectionFaced = Direction.Left;
            Direction = Direction.None;
            Ap.PlayAnimation(_leftRightStaticAnimation);
            this._time = 0;
            this._countBeforeDecrementingEnergy = 0;

            this._weapon1.Reset();
            this._weapon2.Reset();
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

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Player;
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            bool result = false;
            var timeRemaining = gameTime.ElapsedGameTime.TotalSeconds;
            this.OriginalPosition = this.Position;
            while (timeRemaining > 0)
                {
                if (!this.IsMoving && !this.SetDirectionAndDestination(gameTime))
                    break;

                result = true;
                this.TryToCompleteMoveToTarget(ref timeRemaining);
                if (CheckIfEnteredNewWorldArea())
                    this.World.Game.SoundPlayer.Play(GameSound.PlayerEntersNewLevel);
                }

            if (!IsExtant)
                return false;
            
            SetAnimation(result);
            UpdateEnergy(gameTime);

            return result;
            }

        private void SetAnimation(bool isMoving)
            {
            Animation a;
            SpriteEffects se;
            switch (this._currentDirectionFaced)
                {
                case Direction.Left:
                    a = isMoving ? this._leftRightMovingAnimation : this._leftRightStaticAnimation;
                    se = SpriteEffects.None;
                    break;
                case Direction.Right:
                    a = isMoving ? this._leftRightMovingAnimation : this._leftRightStaticAnimation;
                    se = SpriteEffects.FlipHorizontally;
                    break;
                case Direction.Up:
                    a = isMoving ? this._upMovingAnimation : this._upStaticAnimation;
                    se = SpriteEffects.None;
                    break;
                case Direction.Down:
                    a = isMoving ? this._downMovingAnimation : this._downStaticAnimation;
                    se = SpriteEffects.None;
                    break;
                default:
                    throw new InvalidOperationException();
                }
            Ap.PlayAnimation(a);
            Ap.SpriteEffect = se;
            }

        private void UpdateEnergy(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time > TicksOfClock)
                {
                this._time -= TicksOfClock;

                this._countBeforeDecrementingEnergy--;
                if (this._countBeforeDecrementingEnergy > 0) 
                    continue;
                _countBeforeDecrementingEnergy = (((this.Energy >> 1) ^ 0xFF) & 0x7F) + 1;

                if (this.Energy == 0)
                    {
                    UponDeath();
                    return;
                    }

                ReduceEnergy(1);
                }
            }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        private bool SetDirectionAndDestination(GameTime gameTime)
            {
            IPlayerInput playerInput = this.World.Game.PlayerInput;
            playerInput.ProcessInput(gameTime);

            this._weapon1.Fire(this, this.World, playerInput.FireStatus1, this._currentDirectionFaced);
            this._weapon2.Fire(this, this.World, playerInput.FireStatus2, this._currentDirectionFaced);
            var requestedDirection = playerInput.Direction;
            if (requestedDirection == Direction.None)
                return false;
            
            // deal with changing which direction the player faces
            if (requestedDirection != this._currentDirectionFaced)
                {
                this._currentDirectionFaced = requestedDirection;
                this._whenCanMoveInDirectionFaced = gameTime.TotalGameTime.Add(this._delayBeforeMovingInDirectionFaced);
                return false;
                }
            if (gameTime.TotalGameTime < this._whenCanMoveInDirectionFaced)
                return false;
            
            // start new movement
            var result = this.CanMoveInDirection(requestedDirection);
            if (result)
                this.Move(requestedDirection, StandardSpeed);
            return result;
            }

        private bool CheckIfEnteredNewWorldArea()
            {
            int worldAreaId = this.World.GetWorldAreaIdForTilePos(this.TilePosition);
            if (this._listOfWorldAreaIdsVisited.Contains(worldAreaId))
                return false;

            this._listOfWorldAreaIdsVisited.Add(worldAreaId);
            return true;
            }

        public void AddEnergy(int energy)
            {
            if (energy <= 0)
                throw new ArgumentOutOfRangeException("energy", energy, "Must be above 0.");
            this.Energy = Math.Min(this.Energy + energy, 255);
            }

        public override void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException("energyToRemove", energyToRemove, "Must be above 0.");
            
            if (energyToRemove > this.Energy)
                UponDeath();
            else
                this.Energy -= energyToRemove;
            }
        
        public override int InstantlyExpire()
            {
            if (!this.IsExtant)
                return 0;

            UponDeath();
            return 0;
            }

        private void UponDeath()
            {
            System.Diagnostics.Trace.WriteLine("*** Player Died ***");
            this.Energy = 0;
            this._countBeforeDecrementingEnergy = 0;

            this.World.Game.SoundPlayer.Play(GameSound.PlayerDies, SoundEffectFinished);
            this.World.AddBang(this.Position, BangType.Long);
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
        public override void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
            {
            if (!IsExtant)
                return;
            
            // Flip the sprite to face the way we are moving.
            Ap.SpriteEffect = _currentDirectionFaced == Direction.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw that sprite.
            Ap.Draw(gameTime, spriteBatch, Position);
            }

        public override ObjectCapability Capability
            {
            get
                {
                return ObjectCapability.CanPushOrCauseBounceBack;
                }
            }

        protected override decimal StandardSpeed
            {
            get
                {
                return AnimationPlayer.BaseSpeed * 2;
                }
            }
        }
    }
