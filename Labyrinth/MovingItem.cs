using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    internal abstract class MovingItem : StaticItem
        {
        public Direction Direction { get; set;}
        protected float CurrentVelocity { get; set; }
        public Vector2 MovingTowards { get; protected set; }

        public abstract bool Update(GameTime gameTime);

        protected MovingItem(World world, Vector2 position) : base(world, position)
            {
            this.MovingTowards = position;
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

        public virtual PushStatus CanBePushedOrBounced(MovingItem byWhom, Direction direction)
            {
            // only the boulder can be pushed or cause a bounceback
            return PushStatus.No;
            }

        public virtual PushStatus CanBePushed(Direction direction)
            {
            // only the player can be bounced backward
            return PushStatus.No;
            }

        public virtual void BounceBack(Direction direction)
            {
            throw new InvalidOperationException();
            }

        // can a moving object move onto a particular square?
        // the square must not be occupied by an impassable object,
        // and any moveable objects must be able to move off in the same direction

        public bool CanMoveTo(Direction direction, bool canBePushedOrBounced)
            {
            TilePos proposedDestination = TilePos.TilePosFromPosition(this.Position).GetPositionAfterOneMove(direction);
            var objectsOnTile = this.World.GetItemsOnTile(proposedDestination);
            foreach (var item in objectsOnTile)
                {
                if (item.Solidity == ObjectSolidity.Impassable)
                    return false;
                if (item.Solidity == ObjectSolidity.Moveable)
                    {
                    var mi = (MovingItem) item;
                    var canMove = canBePushedOrBounced 
                        ? mi.CanBePushedOrBounced(this, direction)
                        : mi.CanBePushed(direction);
                    if (canMove != PushStatus.Yes && canMove != PushStatus.No)
                        return false;
                    }
                else
                    throw new InvalidOperationException();
                }

            return true;
            }
        }
    }
