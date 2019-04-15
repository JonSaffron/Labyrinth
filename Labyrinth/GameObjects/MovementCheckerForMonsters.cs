namespace Labyrinth.GameObjects
    {
    class MovementCheckerForMonsters : MovementChecker
        {
        protected override bool CanObjectOccupySameTile(IMovingItem gameObject, IGameObject objectAlreadyOnTile, Direction direction, bool isBounceBackPossible)
            {
            if (objectAlreadyOnTile.Properties.Get(GameObjectProperties.DeadlyToTouch))
                return false;
            return base.CanObjectOccupySameTile(gameObject, objectAlreadyOnTile, direction, isBounceBackPossible);
            }
        }
    }
