using System;
using System.Linq;
using System.Collections.Generic;
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
        protected IMonsterMotion? DetermineDirection;
        protected IDynamicAnimation? AnimationPlayer;

        public IBoundMovement? SightBoundary { get; private set; }

        public readonly MonsterDef Definition;

        private IEnumerator<bool>? _movementIterator;
        protected double RemainingTime;
        private ChangeRooms _changeRooms;

        public IMonsterWeapon? Weapon { get; set; }

        public Monster(MonsterDef definition, string textureName, int baseMovesDuringAnimation) : this(definition)
            {
            this.AnimationPlayer = new LoopedAnimation(this, textureName, baseMovesDuringAnimation);
            }

        protected Monster(MonsterDef definition) : base(definition.Position)
            {
            this.Definition = definition;
            this.Energy = definition.Energy;
            this.Behaviours = new BehaviourCollection(this);
            }

        public bool IsActive { get; private set; }

        public void Activate()
            {
            if (!this.IsActive)
                {
                this.IsActive = true;
                SetMonsterMotion(isInitialActivation: true);
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
                this.DetermineDirection = new Stationary(this);
                return;
                }

            if (!this.MovementMethods.TryGetValue(this.Mobility, out var movementType))
                throw new InvalidOperationException("Monster is not set for movement " + this.Mobility);
            var initialDirection = isInitialActivation ? this.Definition.InitialDirection.GetValueOrDefault(Direction.None) : Direction.None;
            this.DetermineDirection = GetImplementationForMotion(this, movementType, initialDirection);
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

        /// <summary>
        /// Update the monster's position, and run its behaviours
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            this.AnimationPlayer?.Update(gameTime);

            // move the monster
            this.RemainingTime = gameTime.ElapsedGameTime.TotalSeconds;
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

        protected void Move(Direction direction)
            {
            base.Move(direction, Constants.BaseSpeed * this.SpeedAdjustmentFactor);
            }

        protected virtual IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            while (true)
                {
                Direction direction = this.GetDirection();
                if (direction == Direction.None)
                    {
                    double timeLeftStationary = Constants.TileLength / (double) (Constants.BaseSpeed * this.SpeedAdjustmentFactor);
                    while (true)
                        {
                        bool hasMoveCompleted = timeLeftStationary <= this.RemainingTime;
                        if (hasMoveCompleted)
                            {
                            this.RemainingTime -= timeLeftStationary;
                            break;  // end of stationary move, so do behaviours and get next direction
                            }

                        timeLeftStationary -= this.RemainingTime;
                        this.RemainingTime = 0;
                        this.Properties.Set(GameObjectProperties.DrawOrder, SpriteDrawOrder.StaticMonster);
                        yield return hasMovedSinceLastCall;
                        hasMovedSinceLastCall = false;
                        }
                    }
                else
                    {
                    Move(direction);
                    while (true)
                        {
                        hasMovedSinceLastCall = true;
                        if (this.TryToCompleteMoveToTarget(ref this.RemainingTime))
                            break;

                        this.Properties.Set(GameObjectProperties.DrawOrder, SpriteDrawOrder.MovingMonster);
                        yield return true; // we have moved
                        }
                    }

                this.Behaviours.Perform<IMovementBehaviour>();
                if (this.RemainingTime <= double.Epsilon * 2)
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        protected Direction GetDirection()
            {
            if (this.DetermineDirection == null)
                throw new InvalidOperationException("Determine Direction object reference not set.");

            var result = this.DetermineDirection.GetDirection();
            return result;
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

        public decimal SpeedAdjustmentFactor { get; set; } = 1m;

        public BehaviourCollection Behaviours { get; }

        public override IRenderAnimation? RenderAnimation => this.AnimationPlayer;

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
                throw new InvalidOperationException($"Type {type.Name} does not implement IMonsterMotion.");
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
                throw new InvalidOperationException($"Failed to get matching constructor information for {type.Name} class.");

            var movementImplementation = (IMonsterMotion) constructorInfo.Invoke(constructorArguments.ToArray());
            return movementImplementation;
            }
        }
    }
