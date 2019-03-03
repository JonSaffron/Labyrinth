using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Monster : MovingItem, IMonster
        {
        public string Breed { get; }
        private MonsterMobility _mobility;
        public readonly Dictionary<MonsterMobility, Type> MovementMethods = new Dictionary<MonsterMobility, Type>();
        [CanBeNull] private IMonsterMotion _determineDirection;
        public Direction InitialDirection = Direction.None;

        private MonsterState _monsterState = MonsterState.Normal;
        [CanBeNull] private GameTimer _hatchingTimer;
        public IBoundMovement SightBoundary { get; set; }

        [NotNull] private readonly BehaviourCollection _behaviours;

        private Animation _normalAnimation;
        private static readonly Animation EggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
        private static readonly Animation HatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);

        public readonly int OriginalEnergy;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;
        private ChangeRooms _changeRooms;
        private bool _isActive;
        [CanBeNull] public IMonsterWeapon Weapon { get; set; }

        public Monster(string breed, AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position)
            {
            this.Breed = breed;
            this.Energy = energy;
            this.OriginalEnergy = energy;
            this.CurrentSpeed = Constants.BaseSpeed;
            this._behaviours = new BehaviourCollection(this);
            }
            

        public bool IsActive
            {
            get => this._isActive;
            set
                {
                this._isActive = value;
                SetMonsterMotion();
                }
            }

        public MonsterMobility Mobility
            {
            get => this._mobility;
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
                var result = this.MonsterState.IsEgg();
                return result;
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
                this.PlaySoundWithCallback(GameSound.EggHatches, EggHatches);
            }

        public void EggHatches(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Normal;
            }

        public void SetNormalAnimation(Animation value)
            {
            this._normalAnimation = value;
            if (this.MonsterState == MonsterState.Normal)
                this.Ap.PlayAnimation(value);
            }

        private MonsterState MonsterState
            {
            get => this._monsterState;
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
            this._determineDirection = this.IsStationary ? new Stationary(this) : GetImplementationForDeterminingDirection(this.Mobility);
            }

        /// <inheritdoc cref="IMonster" />
        public bool IsStationary => !this.IsActive || this.IsEgg || this.Mobility == MonsterMobility.Stationary;

        protected override void UponInjury()
            {
            this._behaviours.Perform<IInjuryBehaviour>();
            }

        protected override void UponDeath()
            {
            var bang = GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            bang.PlaySound(GameSound.MonsterDies);
            this._behaviours.Perform<IDeathBehaviour>();
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

                    this._behaviours.Perform<IMovementBehaviour>();
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
                this._behaviours.Perform<IMovementBehaviour>();
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

        public BehaviourCollection Behaviours => this._behaviours;

        public bool HasBehaviour<T>() where T : IBehaviour
            {
            var result = this._behaviours.Has<T>();
            return result;
            }

        public ChangeRooms ChangeRooms
            {
            get => this._changeRooms;
            set
                {
                this._changeRooms = value;
                if (value.CanChangeRooms())
                    {
                    var worldBoundary = GlobalServices.BoundMovementFactory.GetWorldBoundary();
                    this.MovementBoundary = worldBoundary;
                    if (value == ChangeRooms.FollowsPlayer)
                        {
                        this.SightBoundary = worldBoundary;
                        }
                    else
                        {
                        this.SightBoundary = GlobalServices.BoundMovementFactory.GetBoundedInRoom(this);
                        }
                    }
                else
                    {
                    // todo not sure why I used this instead of GetBoundedInRoom
                    var roomBoundary = GlobalServices.BoundMovementFactory.GetExplicitBoundary(World.GetContainingRoom(this.TilePosition));
                    this.MovementBoundary = roomBoundary;
                    this.SightBoundary = roomBoundary;
                    }
                }
            }

        protected virtual IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            throw new NotImplementedException("This is no longer used.");
            }

        private IMonsterMotion GetImplementationForDeterminingDirection(MonsterMobility mobility)
            {
            Type type = this.MovementMethods[mobility];
            if (!type.GetInterfaces().Contains(typeof(IMonsterMotion)))
                {
                throw new InvalidOperationException("Type " + type.Name + " does not implement IMonsterMotion.");
                }

            var constructorArgTypes = new List<Type> {typeof(Monster)};
            if (this.InitialDirection != Direction.None)
                {
                constructorArgTypes.Add(typeof(Direction));
                }

            var constructorInfo = type.GetConstructor(constructorArgTypes.ToArray());
            if (constructorInfo == null)
                throw new InvalidOperationException("Failed to get matching constructor information for " + type.Name + " class.");

            var constructorArguments = new List<object> {this};
            if (this.InitialDirection != Direction.None)
                {
                constructorArguments.Add(this.InitialDirection);
                }

            var movementImplementation = (IMonsterMotion) constructorInfo.Invoke(constructorArguments.ToArray());
            return movementImplementation;
            }
        }
    }
