using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class MovingItem : StaticItem, IMovingItem
        {
        /// <inheritdoc />
        /// <summary>
        /// Constructs a new MovingItem object
        /// </summary>
        protected MovingItem(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            // nothing to do
            }

        /// <summary>
        /// Returns or sets the current movement by this object
        /// </summary>
        public Labyrinth.Movement CurrentMovement { get; protected set; }

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
            this.CurrentMovement = Labyrinth.Movement.Still;
            GlobalServices.GameState.UpdatePosition(this);
            }

        /// <summary>
        /// Updates the position and/or activity of this object each tick of the game
        /// </summary>
        /// <param name="gameTime">The amount of gametime that has passed since the last time Update was called</param>
        /// <returns>True if the position of the object changes, otherwise false if the object is stationary</returns>
        public abstract bool Update(GameTime gameTime);

        /// <summary>
        /// Determines whether this object can move in the specified direction
        /// </summary>
        /// <param name="direction">The direction to test for</param>
        /// <returns>True if the object is able to begin a movement in the specified direction, even if it might then bounce backwards</returns>
        public bool CanMoveInDirection(Direction direction)
            {
            bool canCauseBounceBack = this.Capability == ObjectCapability.CanPushOrCauseBounceBack;
            var result = this.CanMoveInDirection(direction, canCauseBounceBack);
            return result;
            }

        /// <summary>
        /// Returns whether or not this object can begin to move in the specified direction
        /// </summary>
        /// <param name="direction">The direction to test for</param>
        /// <param name="isBounceBackPossible">Specifies whether the movement is allowed to start a bounce back</param>
        /// <returns>True if the object can start to move in the specified direction</returns>
        /// <remarks>In order to move, the target tile must not be occupied by an impassable object, and 
        /// a moveable object must be able to move off the target tile in the same direction.</remarks>
        public bool CanMoveInDirection(Direction direction, bool isBounceBackPossible)
            {
            TilePos proposedDestination = this.TilePosition.GetPositionAfterOneMove(direction);
            if (!GlobalServices.World.IsTileWithinWorld(proposedDestination))
                return false;
            if (!this.CanChangeRooms)
                {
                var currentRoom = World.GetContainingRoom(this.Position);
                var potentialRoom = World.GetContainingRoom(proposedDestination.ToPosition());
                if (currentRoom != potentialRoom)
                    return false;
                }
            var objectsOnTile = GlobalServices.GameState.GetItemsOnTile(proposedDestination);
            foreach (var item in objectsOnTile)
                {
                switch (item.Solidity)
                    {
                    case ObjectSolidity.Stationary:
                    case ObjectSolidity.Insubstantial:
                        // neither of these will stop this object moving onto the same tile
                        continue;

                    case ObjectSolidity.Impassable:
                        // the target tile is already occupied and this object cannot move onto it
                        return false;

                    case ObjectSolidity.Moveable:
                        {
                        if (!(item is MovingItem mi))
                            // There shouldn't be any Moveable objects that are not MovingItems as that would be a contradiction
                            return false;
                        var canMove = mi.CanBePushedOrBounced(this, direction, isBounceBackPossible);
                        if (canMove == PushStatus.Yes || canMove == PushStatus.Bounce)
                            continue;
                        return false;
                        }

                    default:
                        throw new InvalidOperationException();
                    }
                }

            return true;
            }

        /// <summary>
        /// Used to determine if this object can be pushed or bounced
        /// </summary>
        /// <param name="byWhom">The object that is potentially pushing this object</param>
        /// <param name="direction">Sets the direction in which this object might be moved</param>
        /// <param name="isBounceBackPossible">Sets whether bounceback should be considered</param>
        /// <returns>An indication of whether this object will move and if so, how it will react</returns>
        /// <remarks>Currently only the boulder is moveable</remarks>
        private PushStatus CanBePushedOrBounced(IMovingItem byWhom, Direction direction, bool isBounceBackPossible)
            {
            // if this object is not moveable then the answer's no
            if (this.Solidity != ObjectSolidity.Moveable)
                return PushStatus.No;

            // if the moving object cannot move other objects then the answer's no
            if (!byWhom.Capability.CanMoveAnother())
                return PushStatus.No;

            // check if this object can move in the specified direction
            if (this.CanMoveInDirection(direction, isBounceBackPossible))
                return PushStatus.Yes;

            // if bounceback is not a possibility then the answer's no
            if (byWhom.Capability != ObjectCapability.CanPushOrCauseBounceBack || !isBounceBackPossible)
                return PushStatus.No;

            // this object will be able to bounceback only if the object that is pushing it can move backwards
            var willBounceBack = this.CanMoveInDirection(direction.Reversed(), false);
            var result = willBounceBack ? PushStatus.Bounce : PushStatus.No;
            return result;
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
            this.CurrentMovement = new Labyrinth.Movement(direction, movingTowards, speed);
            }

        /// <summary>
        /// Makes this object stationary
        /// </summary>
        public void StandStill()
            {
            this.CurrentMovement = Labyrinth.Movement.Still;
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
            this.CurrentMovement = new Labyrinth.Movement(direction, movingTowards, speed);
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

        /// <summary>
        /// Returns whether this object is currently moving
        /// </summary>
        public virtual bool IsMoving => this.CurrentMovement.IsMoving;

        /// <inheritdoc />
        public override ObjectSolidity Solidity => ObjectSolidity.Insubstantial;

        /// <summary>
        /// Gets an indication of what effect this object can have on others
        /// </summary>
        public virtual ObjectCapability Capability => ObjectCapability.CannotMoveOthers;

        /// <summary>
        /// Gets the normal speed this object moves at
        /// </summary>
        /// <remarks>Measured in pixels per second</remarks>
        public virtual decimal StandardSpeed => Constants.BaseSpeed;

        /// <summary>
        /// Gets the speed this object moves at when bouncing back
        /// </summary>
        /// <remarks>Measured in pixels per second</remarks>
        protected virtual decimal BounceBackSpeed => Constants.BounceBackSpeed;

        // todo: get rid of this. replace with a boundries property which equals the world rect if the object can change rooms, and the room rect if it cannot.
        public virtual bool CanChangeRooms => false;
        }
    }
