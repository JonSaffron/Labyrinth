using System;
using System.Collections.Generic;
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
        public readonly Dictionary<MonsterMobility, IMonsterMotion> MovementMethods = new Dictionary<MonsterMobility, IMonsterMotion>();
        [CanBeNull] private IMonsterMotion _determineDirection;

        public IBoundMovement SightBoundary { get; private set; }

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
            this.Behaviours = new BehaviourCollection(this);
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

        public void SetMonsterMotion(Direction initialDirection = Direction.None)
            {
            if (this.IsStationary)
                {
                this._determineDirection = new Stationary(this);
                this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Stationary);
                return;
                }

            if (!this.MovementMethods.TryGetValue(this.Mobility, out this._determineDirection))
                throw new InvalidOperationException("Monster is not set for movement " + this.Mobility);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Insubstantial);
            }

        /// <inheritdoc cref="IMonster" />
        public bool IsStationary => !this.IsActive || this.Mobility == MonsterMobility.Stationary;

        protected override void UponInjury()
            {
            this.Behaviours.Perform<IInjuryBehaviour>();
            }

        protected override void UponDeath()
            {
            var bang = GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            bang.PlaySound(GameSound.MonsterDies);
            this.Behaviours.Perform<IDeathBehaviour>();
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
                var timeItWouldTakeToMakeAMove = Constants.TileLength / (double) this.StandardSpeed;
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
        public override decimal StandardSpeed => this.CurrentSpeed;

        [NotNull]
        public BehaviourCollection Behaviours { get; }

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
        }
    }
