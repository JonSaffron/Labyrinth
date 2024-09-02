using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.DataStructures;
using Labyrinth.Services.Display;
using Labyrinth.Services.Messages;
using Microsoft.Xna.Framework;

/* If at grid point: On new keypress
 *  If not facing the same way then turn to face the indicated direction return
 *  Start moving in the direction faced. Cache the direction.
 * If at grid point and key is still pressed
 *  Keep moving in cached direction
 * If moving then:
 *  If key released and haven't reached the next stop point then keep moving in cached direction
 *  If key released and have reached the next stop point then stop moving
 *  If new key pressed change direction being faced. Cache the direction.
 */

namespace Labyrinth.GameObjects
    {
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    public class Player : MovingItem, IPlayer
        {
        // Movement
        public Direction CurrentDirectionFaced { get; private set; } // can't be none
        public TimeSpan WhenCanMoveInDirectionFaced { get; private set; }
        private readonly TimeSpan _delayBeforeMovingInDirectionFaced = TimeSpan.FromMilliseconds(75);
        
        private readonly IPlayerWeapon _weapon1;
        private readonly IPlayerWeapon _weapon2;

        // Animations
        private readonly PlayerAnimation _playerAnimation;

        private readonly HashSet<int> _crystalsCollected = new HashSet<int>();
        private readonly HashSet<int> _worldAreaIdsVisited = new HashSet<int>();

        // used to keep track of game ticks when reducing player's energy over time
        private double _time;
        // number of game ticks before player's energy is decremented
        private int _countBeforeDecrementingEnergy;

        // for movement
        private IEnumerator<bool>? _movementIterator;
        private double _remainingTime;

        // this is set on every update and can be compared against WhenCanMoveInDirectionFaced
        private TimeSpan _totalGameTimeElapsed;

        /// <summary>
        /// Constructs a new player.
        /// </summary>
        public Player(Vector2 position, int energy, int initialWorldAreaId) : base(position)
            {
            // Load animated textures.
            this._playerAnimation = new PlayerAnimation(this);
            
            this._weapon1 = new StandardPlayerWeapon();
            this._weapon2 = new MineLayer();
            this.Energy = energy;
            this._worldAreaIdsVisited.Add(initialWorldAreaId);
            Reset();
            this.MovementBoundary = GlobalServices.BoundMovementFactory.GetWorldBoundary();

            this.Properties.Set(GameObjectProperties.Capability, ObjectCapability.CanPushOrCauseBounceBack);
            this.Properties.Set(GameObjectProperties.DrawOrder, SpriteDrawOrder.Player);
            }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        public void Reset()
            {
            this.CurrentDirectionFaced = Direction.Left;
            this.CurrentMovement = Movement.Still;
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

        public override IRenderAnimation RenderAnimation => this._playerAnimation;

        public override bool IsExtant
            {
            get
                {
                var result = this.Energy > 0 || this._countBeforeDecrementingEnergy > 0;
                return result;
                }
            }

        /// <summary>
        /// Removes the specified amount of energy from the object
        /// </summary>
        /// <param name="energyToRemove">The amount to reduce the object's energy by</param>
        public override void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException(nameof(energyToRemove), energyToRemove, "Must be above 0.");
            if (!this.IsExtant)
                return;

            if (energyToRemove > this.Energy)
                {
                this.Energy = 0;
                this._countBeforeDecrementingEnergy = 0;
                UponDeath(false);
                }
            else
                {
                this.Energy -= energyToRemove;
                UponInjury();
                }
            }

        /// <summary>
        /// Handles input, performs physics
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            if (!IsExtant)
                return false;

            this._playerAnimation.Update(gameTime);

            // todo should the player shooting be dealt with here?

            // move the player
            this._remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            this._totalGameTimeElapsed = gameTime.TotalGameTime;
            if (this._movementIterator == null)
                this._movementIterator = this._movementIterator = Move().GetEnumerator();
            this._movementIterator.MoveNext();
            var result = this._movementIterator.Current;

            UpdateEnergy(gameTime);

            return result;
            }

        private IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            while (true)
                {
                bool hasNewMovementStarted = this.CheckInputForPlayer();
                if (hasNewMovementStarted)
                    {
                    hasMovedSinceLastCall = true;

                    while (true)
                        {
                        bool hasMoveFinished = this.TryToCompleteMoveToTarget(ref this._remainingTime);
                        if (hasMoveFinished)
                            {
                            break;
                            }

                        yield return true;  // indicate that the player object is currently moving
                        }

                    // movement has now finished
                    if (HasPlayerEnteredNewWorldArea(out int worldAreaId))
                        {
                        GlobalServices.SoundPlayer.Play(GameSound.PlayerEntersNewLevel);
                        var msg = new WorldStatus($"Level {worldAreaId}");
                        Messenger.Default.Send(msg);
                        }
                    }
                else
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        public override void Move(Direction direction, MovementSpeed movementSpeed)
            {
            if (movementSpeed == MovementSpeed.BounceBack && this.CurrentMovement.IsMoving)
                {
                this._playerAnimation.ResetAnimation();
                }

            base.Move(direction, movementSpeed);
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
                    UponDeath(false);
                    return;
                    }

                this._countBeforeDecrementingEnergy = ((this.Energy >> 1) ^ 0xFF) & 0x7F;
                ReduceEnergy(1);
                }
            }

        /// <summary>
        /// Updates the player's velocity and position based on input
        /// </summary>
        /// <returns>True if the player starts moving</returns>
        private bool CheckInputForPlayer()
            {
            IPlayerInput playerInput = GlobalServices.PlayerInput;
            var playerControl = playerInput.Update();

            var result = ChangeDirectionFacedOrStartMove(playerControl.Direction);            

            this._weapon1.Fire(this, playerControl.FireState1, this.CurrentDirectionFaced);
            this._weapon2.Fire(this, playerControl.FireState2, this.CurrentDirectionFaced);
            
            return result;
            }

        /// <summary>
        /// May update in which direction the player is facing, or may start moving in the indicated direction
        /// </summary>
        /// <param name="requestedDirection">The requested direction to face or move</param>
        /// <returns>True if the player starts moving</returns>
        private bool ChangeDirectionFacedOrStartMove(Direction requestedDirection)
            {
            if (requestedDirection == Direction.None)
                return false;

            // deal with changing which direction the player faces
            if (requestedDirection != this.CurrentDirectionFaced)
                {
                this.CurrentDirectionFaced = requestedDirection;
                this.WhenCanMoveInDirectionFaced = this._totalGameTimeElapsed.Add(this._delayBeforeMovingInDirectionFaced);
                return false;
                }
            if (this._totalGameTimeElapsed < this.WhenCanMoveInDirectionFaced)
                return false;
            
            // start new movement
            var result = this.CanMoveInDirection(requestedDirection);
            if (result)
                this.Move(requestedDirection, StandardSpeed);
            return result;
            }

        private bool HasPlayerEnteredNewWorldArea(out int currentWorldAreaId)
            {
            currentWorldAreaId = GlobalServices.World.GetWorldAreaIdForTilePos(this.TilePosition);
            bool result = this._worldAreaIdsVisited.Add(currentWorldAreaId);
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
            UponDeath(true);
            }

        protected override void UponDeath(bool wasDeathInstant)
            {
            var deathTilePos = this.CurrentMovement.IsMoving ? TilePos.TilePosFromPosition(this.CurrentMovement.MovingTowards) : this.TilePosition;
            GlobalServices.GameState.AddBang(deathTilePos.ToPosition(), BangType.Long);
            GlobalServices.GameState.AddGrave(deathTilePos);
            Messenger.Default.Send(new LifeLost());
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
        
        private const decimal StandardSpeed = Constants.BaseSpeed * 2;
        }
    }
