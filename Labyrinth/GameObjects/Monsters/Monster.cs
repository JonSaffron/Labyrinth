using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Actions;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

// todo should monsters be a combination of the generic monster class and a breed class?

namespace Labyrinth.GameObjects
    {
    public abstract class Monster : MovingItem, IMonster
        {
        private MonsterMobility _mobility;
        protected abstract IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility);
        [CanBeNull] private IMonsterMotion _determineDirection;
        public Direction InitialDirection = Direction.None;

        public ChangeRooms ChangeRooms { get; set; }
        private MonsterState _monsterState = MonsterState.Normal;
        [CanBeNull] private GameTimer _hatchingTimer;

        public bool IsActive { get; set; }
        public bool ShotsBounceOff { get; set; }

        [NotNull] private readonly BehaviourCollection _movementBehaviours;
        [NotNull] private readonly BehaviourCollection _injuryBehaviours;
        [NotNull] private readonly BehaviourCollection _deathBehaviours;

        private Animation _normalAnimation;
        private static readonly Animation EggAnimation;
        private static readonly Animation HatchingAnimation;

        public readonly int OriginalEnergy;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;

        static Monster()
            {
            EggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
            HatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);
            }

        protected Monster(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position)
            {
            this.Energy = energy;
            this.OriginalEnergy = energy;
            this.CurrentSpeed = Constants.BaseSpeed;
            this._movementBehaviours = new BehaviourCollection(this);
            this._injuryBehaviours = new BehaviourCollection(this);
            this._deathBehaviours = new BehaviourCollection(this);
            }
            
        public MonsterMobility Mobility
            {
            get
                {
                return this._mobility;
                }
            set
                {
                this._mobility = value;
                SetMonsterMotion();
                }
            }

        /// <inheritdoc cref="IMonster" />
        public bool IsEgg
            {
            get
                {
                var monsterState = this.MonsterState;
                switch (monsterState)
                    {
                    case MonsterState.Egg:
                    case MonsterState.Hatching:
                        return true;
                    }
                return false;
                }
            }

        public void SetDelayBeforeHatching(int gameTicks)
            {
            this.MonsterState = MonsterState.Egg;
            var timeSpan = TimeSpan.FromSeconds(gameTicks * Constants.GameClockResolution);
            this._hatchingTimer = GameTimer.AddGameTimer(timeSpan, EggIsHatching, false);
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Hatching;
            if (this.IsExtant)
                PlaySoundWithCallback(GameSound.EggHatches, EggHatches);
            }

        public void EggHatches(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Normal;
            }

        protected void SetNormalAnimation(Animation value)
            {
            this._normalAnimation = value;
            if (this.MonsterState == MonsterState.Normal)
                this.Ap.PlayAnimation(value);
            }

        private MonsterState MonsterState
            {
            get
                {
                var result = this._monsterState;
                return result;
                }
            set
                {
                Animation animation;
                switch (value)
                    {
                    case MonsterState.Egg:
                        animation = EggAnimation;
                        break;
                    case MonsterState.Hatching:
                        animation = HatchingAnimation;
                        break;
                    case MonsterState.Normal:
                        animation = this._normalAnimation;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                this.Ap.PlayAnimation(animation);
                this._monsterState = value;
                SetMonsterMotion();
                }
            }

        private void SetMonsterMotion()
            {
            this._determineDirection = !this.IsActive || this.IsEgg || this.Mobility == MonsterMobility.Stationary ? new Stationary(this) : GetMethodForDeterminingDirection(this.Mobility);
            }

        /// <inheritdoc cref="IMonster" />
        public bool IsStationary => this._determineDirection is Stationary;

        public override void ReduceEnergy(int energyToRemove)
            {
            base.ReduceEnergy(energyToRemove);
            if (this.IsAlive())
                {
                this._injuryBehaviours.PerformAll();
                return;
                }

            var bang = GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            bang.PlaySound(GameSound.MonsterDies);
            
            this.DeathBehaviours.PerformAll();
            }

        public override int DrawOrder
            {
            get
                {
                var result = this.IsMoving ? SpriteDrawOrder.MovingMonster : SpriteDrawOrder.StaticMonster;
                return (int) result;
                }
            }

        private bool SetDirectionAndDestination()
            {
            if (this._determineDirection == null)
                throw new InvalidOperationException("Determine Direction object reference not set.");

            var result = this._determineDirection.SetDirectionAndDestination();
            return result;
            }

        /// <summary>
        /// Update the monster's position, and run its behaviours
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this);

            if (this.IsEgg && this._hatchingTimer != null)
                this._hatchingTimer.Enabled = inSameRoom;

            if (inSameRoom && !this.IsActive)
                {
                this.IsActive = true;
                SetMonsterMotion();
                }

            if (!this.IsActive)
                return false;

            // move the monster
            this._remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            if (this._movementIterator == null)
                this._movementIterator = this._movementIterator = Move().GetEnumerator();
            this._movementIterator.MoveNext();
            var result = this._movementIterator.Current;
            return result;
            }

        public override void ResetPosition(Vector2 position)
            {
            base.ResetPosition(position);
            // it's essential to reset the iterator 
            this._movementIterator = null;
            }

        private IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            double timeStationary = 0;
            while (true)
                {
                if (this.SetDirectionAndDestination())
                    {
                    hasMovedSinceLastCall = true;
                    timeStationary = 0;
                    while (true)
                        {
                        if (this.TryToCompleteMoveToTarget(ref this._remainingTime))
                            break;
                        
                        yield return true;  // we have moved
                        }

                    this.MovementBehaviours.PerformAll();
                    }
                else
                    {
                    timeStationary += this._remainingTime;
                    DoActionsWhilstStationary(ref timeStationary);
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private void DoActionsWhilstStationary(ref double timeStationary)
            {
            while (true)
                {
                var timeItWouldTakeToMakeAMove = Constants.TileLength / (double) this.StandardSpeed;
                if (timeStationary < timeItWouldTakeToMakeAMove)
                    break;

                timeStationary -= timeItWouldTakeToMakeAMove;
                this.MovementBehaviours.PerformAll();
                }
            }

        /// <inheritdoc cref="MovingItem.TryToCompleteMoveToTarget" />
        protected override bool TryToCompleteMoveToTarget(ref double timeRemaining)
            {
            Rectangle currentRoom = World.GetContainingRoom(this.Position);
            
            var result = base.TryToCompleteMoveToTarget(ref timeRemaining);

            Rectangle newRoom = World.GetContainingRoom(this.Position);
            if (currentRoom != newRoom)
                {
                Rectangle playerRoom = World.GetContainingRoom(GlobalServices.GameState.Player.Position);
                if (currentRoom == playerRoom)
                    this.PlaySound(GameSound.MonsterLeavesRoom);
                else if (newRoom == playerRoom)
                    this.PlaySound(GameSound.MonsterEntersRoom);
                }
            return result;
            }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        /// <remarks>The implementation of this must be consistent with the type of monster and its current state (egg/normal).
        /// It cannot look at the current movement class being used because that inactive monsters will be stationary.
        /// The TileContentValidator needs this value to be reflective of the general state of the monster, not its current ephemeral state.</remarks>
        public override ObjectSolidity Solidity => (this.IsEgg || this.Mobility == MonsterMobility.Stationary) ? ObjectSolidity.Stationary : ObjectSolidity.Insubstantial;

        public int CurrentSpeed { get; set; }
        public override decimal StandardSpeed => this.CurrentSpeed;

        [NotNull] public BehaviourCollection MovementBehaviours => this._movementBehaviours;

        [NotNull] public BehaviourCollection InjuryBehaviours => this._injuryBehaviours;

        [NotNull] public BehaviourCollection DeathBehaviours => this._deathBehaviours;

        public bool HasBehaviour<T>() where T : IBehaviour
            {
            var result = this.MovementBehaviours.Has<T>() || this.InjuryBehaviours.Has<T>() || this.DeathBehaviours.Has<T>();
            return result;
            }

        protected override bool CanChangeRooms => this.ChangeRooms == ChangeRooms.FollowsPlayer || this.ChangeRooms == ChangeRooms.MovesRoom;
        }
    }
