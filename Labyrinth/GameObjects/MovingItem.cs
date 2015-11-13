using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class MovingItem : StaticItem
        {
        public Labyrinth.Movement CurrentMovement { get; protected set; }
        public Vector2 OriginalPosition { get; set; }

        public abstract bool Update(GameTime gameTime);

        protected MovingItem(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            // nothing to do
            }

        /// <summary>
        /// Determines whether an object can move in the specified direction
        /// </summary>
        /// <param name="direction">The direction to test for</param>
        /// <returns>True if the object is able to begin a movement in the specified direction, even if it might then bounce backwards</returns>
        public bool CanMoveInDirection(Direction direction)
            {
            var result = this.CanMoveInDirection(direction, true);
            return result;
            }

        /// <summary>
        /// Used to begin a push or bounce action by the player
        /// </summary>
        /// <param name="byWhom">The game object that is acting on the boulder</param>
        /// <param name="direction">Which direction the specified game object is directing the boulder</param>
        public void PushOrBounce(MovingItem byWhom, Direction direction)
            {
            var ps = CanBePushedOrBounced(byWhom, direction, true);
            switch (ps)
                {
                case PushStatus.No:
                    return;

                case PushStatus.Yes:
                    {
                    System.Diagnostics.Trace.WriteLine(string.Format("{0} is pushing {1}", byWhom.GetType().Name, this.GetType().Name));
                    this.Move(direction, this.StandardSpeed);
                    return;
                    }

                case PushStatus.Bounce:
                    {
                    System.Diagnostics.Trace.WriteLine(string.Format("{0} is bouncing {1}", byWhom.GetType().Name, this.GetType().Name));
                    var reverseDirection = direction.Reversed();
                    this.Move(reverseDirection, this.BounceBackSpeed);
                    byWhom.BounceBack(reverseDirection, this.BounceBackSpeed);
                    this.PlaySound(GameSound.BoulderBounces);
                    return;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }

        /// <summary>
        /// Returns whether or not the object can begin to move in the specified direction
        /// </summary>
        /// <param name="direction">The direction to test for</param>
        /// <param name="isBounceBackPossible">Specifies whether the movement is allowed to start a bounce back</param>
        /// <returns>True if the object can start to move in the specified direction</returns>
        /// <remarks>In order to move, the target tile must not be occupied by an impassable object, and 
        /// a moveable object must be able to move off the target tile in the same direction.</remarks>
        private bool CanMoveInDirection(Direction direction, bool isBounceBackPossible)
            {
            TilePos proposedDestination = this.TilePosition.GetPositionAfterOneMove(direction);
            if (!GlobalServices.GameState.IsTileWithinWorld(proposedDestination))
                return false;
            var objectsOnTile = GlobalServices.GameState.GetItemsOnTile(proposedDestination);
            foreach (var item in objectsOnTile)
                {
                switch (item.Solidity)
                    {
                    case ObjectSolidity.Stationary:
                    case ObjectSolidity.Insubstantial:
                        continue;

                    case ObjectSolidity.Impassable:
                        return false;

                    case ObjectSolidity.Moveable:
                        {
                        var mi = item as MovingItem;
                        if (mi == null)
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

        private PushStatus CanBePushedOrBounced(MovingItem byWhom, Direction direction, bool isBounceBackPossible)
            {
            if (this.Solidity != ObjectSolidity.Moveable)
                return PushStatus.No;

            if (!byWhom.Capability.CanMoveAnother())
                return PushStatus.No;

            // first check if the object can be pushed
            if (this.CanMoveInDirection(direction, isBounceBackPossible))
                return PushStatus.Yes;

            // is bounce back possible?
            if (byWhom.Capability != ObjectCapability.CanPushOrCauseBounceBack || !isBounceBackPossible)
                return PushStatus.No;

            var result = byWhom.CanMoveInDirection(direction.Reversed(), false) ? PushStatus.Bounce : PushStatus.No;
            return result;
            }

        /// <summary>
        /// Starts the object moving
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="speed">The speed to move at</param>
        protected void Move(Direction direction, decimal speed)
            {
            var movingTowardsTilePos = this.TilePosition.GetPositionAfterOneMove(direction);
            var movingTowards = movingTowardsTilePos.ToPosition();
            this.CurrentMovement = new Labyrinth.Movement(direction, movingTowards, speed);
            System.Diagnostics.Trace.WriteLine(string.Format("{0}: Moving {1} to {2}", this.GetType().Name, direction, movingTowardsTilePos));
            }

        protected void StandStill()
            {
            this.CurrentMovement = Labyrinth.Movement.Still;
            System.Diagnostics.Trace.WriteLine(string.Format("{0}: Standing still at {1}", this.GetType().Name, this.TilePosition));
            }

        protected void BounceBack(Direction direction, decimal speed)
            {
            var originallyMovingTowards = TilePos.TilePosFromPosition(this.CurrentMovement.MovingTowards);
            var movingTowardsTilePos = originallyMovingTowards.GetPositionAfterMoving(direction, 2);
            var movingTowards = movingTowardsTilePos.ToPosition();
            this.CurrentMovement = new Labyrinth.Movement(direction, movingTowards, speed);
            System.Diagnostics.Trace.WriteLine(string.Format("{0}: Bouncing back {1} to {2}", this.GetType().Name, direction, movingTowardsTilePos));
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
                throw new ArgumentOutOfRangeException("timeRemaining");
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

        public virtual bool IsMoving
            {
            get
                {
                var result = this.CurrentMovement.IsMoving;
                return result;
                }
            }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        public override ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Insubstantial;
                }
            }

        /// <summary>
        /// Gets an indication of what effect this object can have on others
        /// </summary>
        public virtual ObjectCapability Capability
            {
            get
                {
                return ObjectCapability.CannotMoveOthers;
                }
            }

        protected virtual decimal StandardSpeed
            {
            get
                {
                return Constants.BaseSpeed;
                }
            }

        protected virtual decimal BounceBackSpeed
            {
            get
                {
                return Constants.BounceBackSpeed;
                }
            }
        }
    }
