using System;
using System.Collections.Generic;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    public class MovementChecker : IMovementChecker
        {
        public bool CanMoveForwards(IMovingItem source, Direction direction)
            {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            bool canCauseBounceBack = source.Properties.Get(GameObjectProperties.Capability) == ObjectCapability.CanPushOrCauseBounceBack;
            var result = CanMove(source, direction, canCauseBounceBack);
            return result;
            }

        public bool CanBePushedBackDueToBounceBack(IMovingItem gameObject, Direction direction)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            return CanMove(gameObject, direction, false);
            }

        private bool CanMove(IMovingItem objectToCheck, Direction direction, bool isBounceBackPossible)
            {
            if (objectToCheck.MovementBoundary == null)
                {
                throw new InvalidOperationException("MovementBoundary is not set for " + objectToCheck + ".");
                }
                
            TilePos proposedDestination = objectToCheck.TilePosition.GetPositionAfterOneMove(direction);
            if (!objectToCheck.MovementBoundary.IsPositionWithinBoundary(proposedDestination))
                {
                return false;
                }

            var result = CanMoveTo(objectToCheck, direction, proposedDestination, isBounceBackPossible);
            return result;
            }

        private bool CanMoveTo(IMovingItem objectToCheck, Direction direction, TilePos proposedDestination, bool isBounceBackPossible)
            {
            var objectsOnTile = GlobalServices.GameState.GetItemsOnTile(proposedDestination);
            var result = CanObjectOccupySameTile(objectToCheck, objectsOnTile, direction, isBounceBackPossible);
            return result;
            }

        private bool CanObjectOccupySameTile(IMovingItem gameObject, IEnumerable<IGameObject> objectsOnTile, Direction direction, bool isBounceBackPossible)
            {
            foreach (var item in objectsOnTile)
                {
                if (!CanObjectOccupySameTile(gameObject, item, direction, isBounceBackPossible))
                    return false;
                }

            return true;
            }

        protected virtual bool CanObjectOccupySameTile(IMovingItem gameObject, IGameObject objectAlreadyOnTile, Direction direction, bool isBounceBackPossible)
            {
            var solidity = objectAlreadyOnTile.Properties.Get(GameObjectProperties.Solidity);
            switch (solidity)
                {
                case ObjectSolidity.Stationary:
                case ObjectSolidity.Insubstantial:
                    // neither of these will stop this object moving onto the same tile
                    return true;

                case ObjectSolidity.Impassable:
                    // the target tile is already occupied and this object cannot move onto it
                    return false;

                case ObjectSolidity.Moveable:
                    {
                    // todo instead of using the same movementchecker, we need to use the movement checker of the boulder instead
                    if (!(objectAlreadyOnTile is MovingItem moveableItem))
                        // There shouldn't be any Moveable objects that are not MovingItems as that would be a contradiction
                        return false;
                    var canMove = CanBePushedOrBounced(moveableItem, gameObject, direction, isBounceBackPossible);
                    if (canMove == PushStatus.Yes || canMove == PushStatus.Bounce)
                        return true;
                    return false;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }

        public PushStatus CanBePushedOrBounced(IMovingItem toBeMoved, IMovingItem byWhom, Direction direction, bool isBounceBackPossible)
            {
            // if this object is not moveable then the answer's no
            if (toBeMoved.Properties.Get(GameObjectProperties.Solidity) != ObjectSolidity.Moveable)
                return PushStatus.No;

            // if the moving object cannot move other objects then the answer's no
            // the moving object cannot be a moveable object - moveable objects can't push other moveable objects
            if (!byWhom.Properties.Get(GameObjectProperties.Capability).CanMoveAnother() || byWhom.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Moveable)
                return PushStatus.No;

            // check if this object can move in the specified direction
            if (CanMove(toBeMoved, direction, isBounceBackPossible))
                return PushStatus.Yes;

            // if bounceback is not a possibility then the answer's no
            if (byWhom.Properties.Get(GameObjectProperties.Capability) != ObjectCapability.CanPushOrCauseBounceBack || !isBounceBackPossible)
                return PushStatus.No;

            // this object will be able to bounceback only if the object that is pushing it can move backwards
            IMovementChecker mc = byWhom.Properties.Get(GameObjectProperties.MovementChecker);
            var willBounceBack = mc.CanBePushedBackDueToBounceBack(byWhom, direction.Reversed());
            var result = willBounceBack ? PushStatus.Bounce : PushStatus.No;
            return result;
            }
        }
    }
