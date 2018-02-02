using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.Services.Display;
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
    public class Player : MovingItem
        {
        // Movement
        public Direction CurrentDirectionFaced { get; private set; } // can't be none
        public TimeSpan WhenCanMoveInDirectionFaced { get; private set; }
        private readonly TimeSpan _delayBeforeMovingInDirectionFaced = TimeSpan.FromMilliseconds(75);
        
        [NotNull] private readonly IPlayerWeapon _weapon1;
        [NotNull] private readonly IPlayerWeapon _weapon2;

        // Animations
        private readonly Animation _leftRightMovingAnimation;
        private readonly Animation _upMovingAnimation;
        private readonly Animation _downMovingAnimation;
        private readonly Animation _leftRightStaticAnimation;
        private readonly Animation _upStaticAnimation;
        private readonly Animation _downStaticAnimation;

        private readonly HashSet<int> _crystalsCollected = new HashSet<int>();
        private readonly HashSet<int> _worldAreaIdsVisited = new HashSet<int>();

        private double _time;
        private int _countBeforeDecrementingEnergy;

        private bool _whichFootFlag;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;
        private GameTime _gameTime;

        /// <summary>
        /// Constructs a new player.
        /// </summary>
        public Player(AnimationPlayer animationPlayer, Vector2 position, int energy, int initialWorldAreaId) : base(animationPlayer, position)
            {
            // Load animated textures.
            this._leftRightMovingAnimation = Animation.LoopingAnimation("Sprites/Player/PlayerLeftFacing", 1);
            this._upMovingAnimation = Animation.LoopingAnimation("Sprites/Player/PlayerUpFacing", 1);
            this._downMovingAnimation = Animation.LoopingAnimation("Sprites/Player/PlayerDownFacing", 1);
            this._leftRightStaticAnimation = Animation.StaticAnimation("Sprites/Player/PlayerLeftFacing");
            this._upStaticAnimation = Animation.StaticAnimation("Sprites/Player/PlayerUpFacing");
            this._downStaticAnimation = Animation.StaticAnimation("Sprites/Player/PlayerDownFacing");

            Ap.NewFrame += PlayerSpriteNewFrame;
            
            this._weapon1 = new StandardPlayerWeapon();
            this._weapon2 = new MineLayer();
            this.Energy = energy;
            this._worldAreaIdsVisited.Add(initialWorldAreaId);
            Reset();
            }

        private void PlayerSpriteNewFrame(object sender, EventArgs e)
            {
            var playerMoves = _whichFootFlag ? GameSound.PlayerMovesFirstFoot : GameSound.PlayerMovesSecondFoot;
            this.PlaySound(playerMoves);
            this._whichFootFlag = !(this._whichFootFlag);
            }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        public void Reset()
            {
            this.CurrentDirectionFaced = Direction.Left;
            this.CurrentMovement = Labyrinth.Movement.Still;
            Ap.PlayAnimation(_leftRightStaticAnimation);
            this._time = 0;

            this._weapon1.Reset();
            this._weapon2.Reset();
            }

        /// <summary>
        /// Resets the player's position and energy level to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        /// <param name="energy">Initial energy</param>
        public void ResetPositionAndEnergy(Vector2 position, int energy)
            {
            ResetPosition(position);
            this.Energy = energy;
            this._countBeforeDecrementingEnergy = 0;
            }

        public override void ResetPosition(Vector2 position)
            {
            base.ResetPosition(position);
            // it's essential to reset the iterator 
            this._movementIterator = null;
            }

        public override bool IsExtant
            {
            get
                {
                var result = this.Energy > 0 || this._countBeforeDecrementingEnergy > 0;
                return result;
                }
            }

        public override int DrawOrder => (int) SpriteDrawOrder.Player;

        /// <summary>
        /// Handles input, performs physics
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            if (!IsExtant)
                return false;

            // todo should the player shooting be dealt with here?

            // move the player
            this._remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            this._gameTime = gameTime;
            if (this._movementIterator == null)
                this._movementIterator = this._movementIterator = Move().GetEnumerator();
            this._movementIterator.MoveNext();
            var result = this._movementIterator.Current;

            SetAnimation(result);
            UpdateEnergy(gameTime);

            return result;
            }

        private IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            while (true)
                {
                if (this.SetDirectionAndDestination())
                    {
                    hasMovedSinceLastCall = true;
                    while (true)
                        {
                        if (this.TryToCompleteMoveToTarget(ref this._remainingTime))
                            break;

                        yield return true;
                        }

                    if (HasPlayerEnteredNewWorldArea())
                        GlobalServices.SoundPlayer.Play(GameSound.PlayerEntersNewLevel);
                    }
                else
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private void SetAnimation(bool isMoving)
            {
            Animation a;
            SpriteEffects se;
            switch (this.CurrentDirectionFaced)
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
            while (this._time > Constants.GameClockResolution)
                {
                this._time -= Constants.GameClockResolution;
                this._countBeforeDecrementingEnergy--;
                if (this._countBeforeDecrementingEnergy > 0) 
                    continue;

                if (!this.IsAlive())
                    {
                    UponDeath();
                    return;
                    }

                this._countBeforeDecrementingEnergy = ((this.Energy >> 1) ^ 0xFF) & 0x7F;
                ReduceEnergy(1);
                }
            }

        /// <summary>
        /// Updates the player's velocity and position based on input
        /// </summary>
        private bool SetDirectionAndDestination()
            {
            IPlayerInput playerInput = GlobalServices.PlayerInput;
            playerInput.ProcessInput();

            var result = SetDirection(playerInput);            

            this._weapon1.Fire(this, playerInput.FireStatus1, this.CurrentDirectionFaced);
            this._weapon2.Fire(this, playerInput.FireStatus2, this.CurrentDirectionFaced);
            
            return result;
            }

        private bool SetDirection(IPlayerInput playerInput)
            {
            var requestedDirection = playerInput.Direction;
            if (requestedDirection == Direction.None)
                return false;

            // deal with changing which direction the player faces
            if (requestedDirection != this.CurrentDirectionFaced)
                {
                this.CurrentDirectionFaced = requestedDirection;
                this.WhenCanMoveInDirectionFaced = this._gameTime.TotalGameTime.Add(this._delayBeforeMovingInDirectionFaced);
                return false;
                }
            if (this._gameTime.TotalGameTime < this.WhenCanMoveInDirectionFaced)
                return false;
            
            // start new movement
            var result = this.CanMoveInDirection(requestedDirection);
            if (result)
                this.Move(requestedDirection, StandardSpeed);
            return result;
            }

        private bool HasPlayerEnteredNewWorldArea()
            {
            int worldAreaId = GlobalServices.World.GetWorldAreaIdForTilePos(this.TilePosition);
            bool result = this._worldAreaIdsVisited.Add(worldAreaId);
            return result;
            }

        public void AddEnergy(int energy)
            {
            if (energy <= 0)
                throw new ArgumentOutOfRangeException(nameof(energy), energy, "Must be above 0.");
            this.Energy = Math.Min(this.Energy + energy, 255);
            }

        public override void InstantlyExpire()
            {
            if (!this.IsExtant)
                return;
            
            this.Energy = 0;
            this._countBeforeDecrementingEnergy = 0;
            UponDeath();
            }

        protected override void UponDeath()
            {
            GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerDies, SoundEffectFinished);
            GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            GlobalServices.GameState.AddGrave(this.TilePosition);
            }

        private void SoundEffectFinished(object sender, EventArgs args)
            {
            GlobalServices.World.SetLevelReturnType(LevelReturnType.LostLife);
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
        
        public override ObjectCapability Capability => ObjectCapability.CanPushOrCauseBounceBack;

        public override decimal StandardSpeed => Constants.BaseSpeed * 2;

        protected override bool CanChangeRooms => true;
        }
    }
