using System;
using Labyrinth.DataStructures;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class MovingItem : StaticItem, IMovingItem
        {
        /// <inheritdoc />
        /// <summary>
        /// Constructs a new MovingItem object
        /// </summary>
        protected MovingItem(Vector2 position) : base(position)
            {
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Insubstantial);
            }

        /// <summary>
        /// Returns or sets the current movement by this object
        /// </summary>
        public Movement CurrentMovement { get; protected set; }

        /// <summary>
        /// Returns or sets the previous position of this object
        /// </summary>
        /// <remarks>Used by the <see cref="GameObjectCollection" /> to keep track of where the object has been</remarks>
        public Vector2 OriginalPosition { get; set; }

        /// <summary>
        /// Arbitrarily sets the position of this object
        /// </summary>
        /// <param name="position">The new position for this object</param>
        /// <remarks>The object will also be set to stationary</remarks>
        public virtual void ResetPosition(Vector2 position)
            {
            this.Position = position;
            this.CurrentMovement = Movement.Still;
            GlobalServices.GameState.UpdatePosition(this);
            }

        /// <summary>
        /// Starts this object moving towards a single adjacent tile
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="speed">The speed to move at</param>
        public void Move(Direction direction, decimal speed)
            {
            var movingTowardsTilePos = this.TilePosition.GetPositionAfterOneMove(direction);
            var movingTowards = movingTowardsTilePos.ToPosition();
            this.CurrentMovement = new Movement(direction, movingTowards, speed);
            }

        /// <summary>
        /// Makes this object stationary
        /// </summary>
        public void StandStill()
            {
            this.CurrentMovement = Movement.Still;
            }

        /// <summary>
        /// Makes this object bounce backwards two tiles from where it is currently headed towards
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="speed">The speed to move at</param>
        /// <remarks>This is used by an object that can move another, currently this will only be the player</remarks>
        public void BounceBack(Direction direction, decimal speed)
            {
            var originallyMovingTowards = TilePos.TilePosFromPosition(this.CurrentMovement.MovingTowards);
            var movingTowardsTilePos = originallyMovingTowards.GetPositionAfterMoving(direction, 2);
            var movingTowards = movingTowardsTilePos.ToPosition();
            this.CurrentMovement = new Movement(direction, movingTowards, speed);
            }

        /// <summary>
        /// Moves a sprite from its current position towards its destination
        /// </summary>
        /// <param name="timeRemaining">On entry: specifies the amount of time that the sprite can use to travel. On exit: specifies how much of that time remains</param>
        /// <returns>True if the object has been able to complete a move to the tile at MovingTowards, or False if it ran out of time during the movement.</returns>
        /// <remarks>On exit, when returning True, the timeRemaining parameter will return a positive value, 
        /// otherwise timeRemaining will return 0.</remarks>
        protected virtual bool TryToCompleteMoveToTarget(ref double timeRemaining)
            {
            if (timeRemaining <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeRemaining));
            if (!this.CurrentMovement.IsMoving)
                throw new InvalidOperationException("Not currently moving.");

            var timeToReachDestination = Vector2.Distance(this.Position, this.CurrentMovement.MovingTowards) / (double) this.CurrentMovement.Velocity;
            bool hasArrivedAtDestination = (timeToReachDestination <= timeRemaining);
            if (hasArrivedAtDestination)
                {
                timeRemaining -= timeToReachDestination;
                this.Position = this.CurrentMovement.MovingTowards;
                StandStill();
                }
            else
                {
                var vectorBetweenPoints = this.CurrentMovement.MovingTowards - this.Position;
                var unitVectorOfTravel = Vector2.Normalize(vectorBetweenPoints);
                var distanceToTravel = (float) (timeRemaining * (float) this.CurrentMovement.Velocity);
                var displacement = unitVectorOfTravel * distanceToTravel;
                this.Position += displacement;
                timeRemaining = 0;
                }
            return hasArrivedAtDestination;
            }

        public IBoundMovement MovementBoundary { get; protected set; }
        }
    }
