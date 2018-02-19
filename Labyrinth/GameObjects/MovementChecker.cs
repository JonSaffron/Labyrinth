using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects
    {
    public class MovementChecker
        {
        protected readonly IMovingItem Source;

        public MovementChecker([NotNull] IMovingItem source)
            {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            }

        public bool CanMove(Direction direction)
            {
            bool canCauseBounceBack = this.Source.Capability == ObjectCapability.CanPushOrCauseBounceBack;
            var result = this.CanMove(this.Source, direction, canCauseBounceBack);
            return result;
            }

        private bool CanMove(IMovingItem objectToCheck, Direction direction, bool isBounceBackPossible)
            {
            TilePos proposedDestination = objectToCheck.TilePosition.GetPositionAfterOneMove(direction);
            if (!IsPositionWithinMovementBoundaries(proposedDestination))
                {
                return false;
                }

            var objectsOnTile = GlobalServices.GameState.GetItemsOnTile(proposedDestination);
            var result = CanObjectOccupySameTile(objectToCheck, objectsOnTile, direction, isBounceBackPossible);
            return result;
            }

        private bool IsPositionWithinMovementBoundaries(TilePos proposedDestination)
            {
            if (!GlobalServices.World.IsTileWithinWorld(proposedDestination))
                return false;
            if (!this.Source.CanChangeRooms)
                {
                var currentRoom = World.GetContainingRoom(this.Source.Position);
                var potentialRoom = World.GetContainingRoom(proposedDestination.ToPosition());
                if (currentRoom != potentialRoom)
                    return false;
                }
            return true;
            }

        private bool CanObjectOccupySameTile(IMovingItem gameObject, IEnumerable<IGameObject> objectsOnTile, Direction direction, bool isBounceBackPossible)
            {
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
                        var canMove = CanBePushedOrBounced(mi, gameObject, direction, isBounceBackPossible);
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

        protected PushStatus CanBePushedOrBounced(IMovingItem toBeMoved, IMovingItem byWhom, Direction direction, bool isBounceBackPossible)
            {
            // if this object is not moveable then the answer's no
            if (toBeMoved.Solidity != ObjectSolidity.Moveable)
                return PushStatus.No;

            // if the moving object cannot move other objects then the answer's no
            // the moving object cannot be a moveable object - moveable objects can't push other moveable objects
            if (!byWhom.Capability.CanMoveAnother() || byWhom.Solidity == ObjectSolidity.Moveable)
                return PushStatus.No;

            // check if this object can move in the specified direction
            if (CanMove(toBeMoved, direction, isBounceBackPossible))
                return PushStatus.Yes;

            // if bounceback is not a possibility then the answer's no
            if (byWhom.Capability != ObjectCapability.CanPushOrCauseBounceBack || !isBounceBackPossible)
                return PushStatus.No;

            // this object will be able to bounceback only if the object that is pushing it can move backwards
            var willBounceBack = CanMove(this.Source, direction.Reversed(), false);
            var result = willBounceBack ? PushStatus.Bounce : PushStatus.No;
            return result;
            }
        }
    }
