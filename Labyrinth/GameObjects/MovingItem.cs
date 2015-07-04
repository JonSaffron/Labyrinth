using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    internal abstract class MovingItem : StaticItem
        {
        public Direction Direction { get; set;}
        public Vector2 MovingTowards { get; protected set; }
        public Vector2 OriginalPosition { get; protected set; }
        protected decimal CurrentVelocity { get; set; }

        public abstract bool Update(GameTime gameTime);

        protected MovingItem(World world, Vector2 position) : base(world, position)
            {
            this.MovingTowards = position;
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
            if (this.CurrentVelocity == 0)
                throw new InvalidOperationException("Object has no velocity.");

            var timeToReachDestination = Vector2.Distance(this.Position, this.MovingTowards) / (double) this.CurrentVelocity;
            bool hasArrivedAtDestination = (timeToReachDestination <= timeRemaining);
            if (hasArrivedAtDestination)
                {
                timeRemaining -= timeToReachDestination;
                this.Position = this.MovingTowards;
                StandStill();
                }
            else
                {
                var vectorBetweenPoints = this.MovingTowards - this.Position;
                var unitVectorOfTravel = Vector2.Normalize(vectorBetweenPoints);
                var distanceToTravel = (float) (timeRemaining * (float) this.CurrentVelocity);
                var displacement = unitVectorOfTravel * distanceToTravel;
                this.Position += displacement;
                timeRemaining = 0;
                }
            return hasArrivedAtDestination;
            }

        private PushStatus CanBePushedOrBounced(MovingItem byWhom, Direction direction, bool isBounceBackPossible)
            {
            if (this.Solidity != ObjectSolidity.Moveable)
                return PushStatus.No;

            bool isPushable = byWhom.Capability == ObjectCapability.CanPushOthers || byWhom.Capability == ObjectCapability.CanPushOrCauseBounceBack;
            if (!isPushable)
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
        /// Determines whether an object can move in the specified direction
        /// </summary>
        /// <param name="direction">The direction to test for</param>
        /// <returns>True if the object is able to begin a movement in the specified direction, even if it might then bounce backwards</returns>
        internal bool CanMoveInDirection(Direction direction)
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
                    byWhom.Move(reverseDirection, this.BounceBackSpeed);
                    this.World.Game.SoundPlayer.Play(GameSound.BoulderBounces);
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
            if (!this.World.IsTileWithinWorld(proposedDestination))
                return false;
            var objectsOnTile = this.World.GameObjects.GetItemsOnTile(proposedDestination);
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

        /// <summary>
        /// Starts the object moving
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="speed">The speed to move at</param>
        protected void Move(Direction direction, decimal speed)
            {
            System.Diagnostics.Trace.WriteLine(string.Format("{0}: Moving {1}", this.GetType().Name, direction));
            this.Direction = direction;
            this.MovingTowards = this.TilePosition.GetPositionAfterOneMove(direction).ToPosition();
            this.CurrentVelocity = speed;
            }

        protected void StandStill()
            {
            this.Direction = Direction.None;
            this.CurrentVelocity = 0;
            }

        public bool IsMoving
            {
            get
                {
                var result = this.Direction != Direction.None && this.CurrentVelocity != 0 && this.MovingTowards != this.Position;
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
                return AnimationPlayer.BaseSpeed;
                }
            }

        protected virtual decimal BounceBackSpeed
            {
            get
                {
                return AnimationPlayer.BounceBackSpeed;
                }
            }
        }
    }
