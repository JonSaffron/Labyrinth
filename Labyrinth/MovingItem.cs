using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    internal abstract class MovingItem : StaticItem
        {
        public Direction Direction { get; set;}
        protected float CurrentVelocity { get; set; }
        public Vector2 MovingTowards { get; protected set; }
        protected ObjectCapability ObjectCapability { get; set; }

        public abstract bool Update(GameTime gameTime);

        protected MovingItem(World world, Vector2 position) : base(world, position)
            {
            this.MovingTowards = position;
            this.ObjectCapability = ObjectCapability.CannotMoveOthers;
            }

        /// <summary>
        /// Moves a sprite from its current position towards its destination
        /// </summary>
        /// <param name="maxMovementRemaining">On entry: specifies the maximum distance that the sprite can travel. On exit: specifies how much of that distance remains</param>
        /// <param name="hasArrivedAtDestination">On exit: specifies whether the sprite has arrived at the point it was moving towards</param>
        /// <remarks>On exit, if hasArrivedAtDestination is set, then maxMovementRemaining will be above 0, 
        /// otherwise hasArrivedAtDestination will be false, and maxMovementRemaining will be 0.</remarks>
        protected virtual void ContinueMove(ref float maxMovementRemaining, out bool hasArrivedAtDestination)
            {
            if (maxMovementRemaining <= 0)
                throw new ArgumentOutOfRangeException("maxMovementRemaining");

            var distanceToDestination = Vector2.Distance(this.Position, this.MovingTowards);
            hasArrivedAtDestination = (distanceToDestination <= maxMovementRemaining);
            if (hasArrivedAtDestination)
                {
                maxMovementRemaining -= distanceToDestination;
                this.Position = this.MovingTowards;
                }
            else
                {
                var vectorBetweenPoints = this.MovingTowards - this.Position;
                var unitVectorOfTravel = Vector2.Normalize(vectorBetweenPoints);
                var displacement = unitVectorOfTravel * maxMovementRemaining;
                this.Position += displacement;
                maxMovementRemaining = 0;
                }
            }

        protected PushStatus CanBePushed(Direction direction)
            {
            var result = this.CanMoveTo(direction) ? PushStatus.Yes : PushStatus.No;
            return result;
            }

        protected PushStatus CanBePushedOrBounced(MovingItem byWhom, Direction direction)
            {
            var result = CanBePushed(direction);
            if (result == PushStatus.No)
                {
                result = byWhom.CanBePushed(direction.Reversed());
                if (result == PushStatus.Yes)
                    result = PushStatus.Bounce;
                }
            return result;
            }

        public virtual void BounceBack(Direction direction)
            {
            throw new InvalidOperationException();
            }

        public bool IsBouncingBack
            {
            get
                {
                var result = this.Direction != Direction.None && Math.Abs(this.CurrentVelocity - AnimationPlayer.BounceBackSpeed) < 1.0;
                return result;
                }
            }

        // can a moving object move onto a particular square?
        // the square must not be occupied by an impassable object,
        // and any moveable objects must be able to move off in the same direction

        protected internal bool CanMoveTo(Direction direction)
            {
            TilePos proposedDestination = this.TilePosition.GetPositionAfterOneMove(direction);
            var objectsOnTile = this.World.GetItemsOnTile(proposedDestination);
            foreach (var item in objectsOnTile)
                {
                switch (item.Solidity)
                    {
                    case ObjectSolidity.Passable:
                        continue;

                    case ObjectSolidity.Impassable:
                        return false;

                    case ObjectSolidity.Moveable:
                        {
                        var mi = item as MovingItem;
                        if (mi == null || this.ObjectCapability == ObjectCapability.CannotMoveOthers)
                            return false;
                        bool canCauseBounceBack = this.ObjectCapability == ObjectCapability.CanPushOrCauseBounceBack && !this.IsBouncingBack;
                        var canMove = canCauseBounceBack 
                            ? mi.CanBePushedOrBounced(this, direction)
                            : mi.CanBePushed(direction);
                        if (canMove == PushStatus.No)
                            return false;
                        continue;
                        }

                    default:
                        throw new InvalidOperationException();
                    }
                }

            return true;
            }
        }
    }
