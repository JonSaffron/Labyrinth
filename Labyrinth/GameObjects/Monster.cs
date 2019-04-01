using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Behaviour;
using Labyrinth.GameObjects.Motility;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Monster : MovingItem, IMonster
        {
        private MonsterMobility _mobility;
        public readonly Dictionary<MonsterMobility, Type> MovementMethods = new Dictionary<MonsterMobility, Type>();
        [CanBeNull] private IMonsterMotion _determineDirection;
        private readonly LoopedAnimation _animationPlayer;

        public IBoundMovement SightBoundary { get; private set; }

        public readonly MonsterDef Definition;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;
        private ChangeRooms _changeRooms;
        private bool _isActive;
        [CanBeNull] public IMonsterWeapon Weapon { get; set; }

        public Monster(MonsterDef definition, string textureName, int baseMovesDuringAnimation) : base(definition.Position)
            {
            this.Definition = definition;
            this._animationPlayer = new LoopedAnimation(this, textureName, baseMovesDuringAnimation);
            this.Energy = definition.Energy;
            this.CurrentSpeed = Constants.BaseSpeed;
            this.Behaviours = new BehaviourCollection(this);
            }
            
        public bool IsActive
            {
            get => this._isActive;
            protected set => this._isActive = value;
            }

        public void Activate()
            {
            if (!this.IsActive)
                {
                this._isActive = true;
                SetMonsterMotion(true);
                }
            }

        public MonsterMobility Mobility
            {
            get => this._mobility;
            set
                {
                this._mobility = value;
                SetMonsterMotion();
                this.Properties.Set(GameObjectProperties.Solidity,
                    value == MonsterMobility.Stationary ? ObjectSolidity.Stationary : ObjectSolidity.Insubstantial);
                }
            }

        public void SetMonsterMotion(bool isInitialActivation = false)
            {
            if (this.IsStationary)
                {
                this._determineDirection = new Stationary(this);
                return;
                }

            if (!this.MovementMethods.TryGetValue(this.Mobility, out var movementType))
                throw new InvalidOperationException("Monster is not set for movement " + this.Mobility);
            var initialDirection = isInitialActivation ? this.Definition.InitialDirection.GetValueOrDefault(Direction.None) : Direction.None;
            this._determineDirection = GetImplementationForMotion(this, movementType, initialDirection);
            }

        /// <inheritdoc cref="IMonster" />
        public bool IsStationary => !this.IsActive || this.Mobility == MonsterMobility.Stationary;

        protected override void UponInjury()
            {
            this.Behaviours.Perform<IInjuryBehaviour>();
            }

        protected override void UponDeath(bool wasDeathInstant)
            {
            var bang = GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            bang.PlaySound(GameSound.MonsterDies);
            if (!wasDeathInstant)
                {
                // In the original game, monsters that are crushed do not spawn
                // Similarly, we don't want to spawn if we are removing all monsters when the last crystal is collected
                this.Behaviours.Perform<IDeathBehaviour>();
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
            this._animationPlayer.Update(gameTime);

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
                        
                        this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.MovingMonster);
                        yield return true;  // we have moved
                        }

                    this.Behaviours.Perform<IMovementBehaviour>();
                    }
                else
                    {
                    timeStationary += this._remainingTime;
                    DoActionsWhilstStationary(ref timeStationary);
                    this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticMonster);
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
                var timeItWouldTakeToMakeAMove = Constants.TileLength / (double) this.CurrentSpeed;
                if (timeStationary < timeItWouldTakeToMakeAMove)
                    break;

                timeStationary -= timeItWouldTakeToMakeAMove;
                this.Behaviours.Perform<IMovementBehaviour>();
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

        public int CurrentSpeed { get; set; }

        [NotNull]
        public BehaviourCollection Behaviours { get; }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

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
                    this.SightBoundary = value == ChangeRooms.FollowsPlayer ? worldBoundary : GlobalServices.BoundMovementFactory.GetBoundedInRoom(this);
                    }
                else
                    {
                    var roomBoundary = GlobalServices.BoundMovementFactory.GetBoundedInRoom(this);
                    this.MovementBoundary = roomBoundary;
                    this.SightBoundary = roomBoundary;
                    }
                }
            }

        private static IMonsterMotion GetImplementationForMotion(Monster monster, Type type, Direction initialDirection)
            {
            if (!type.GetInterfaces().Contains(typeof(IMonsterMotion)))
                {
                throw new InvalidOperationException("Type " + type.Name + " does not implement IMonsterMotion.");
                }

            var constructorArgTypes = new List<Type> {typeof(Monster)};
            var constructorArguments = new List<object> {monster};
            if (initialDirection != Direction.None)
                {
                constructorArgTypes.Add(typeof(Direction));
                constructorArguments.Add(initialDirection);
                }

            var constructorInfo = type.GetConstructor(constructorArgTypes.ToArray());
            if (constructorInfo == null)
                throw new InvalidOperationException("Failed to get matching constructor information for " + type.Name + " class.");

            var movementImplementation = (IMonsterMotion) constructorInfo.Invoke(constructorArguments.ToArray());
            return movementImplementation;
            }
        }
    }
