
namespace Labyrinth
    {
    public interface IMovementChecker
        {
        bool CanMoveForwards(IMovingItem source, Direction direction);
        bool CanBePushedBackDueToBounceBack(IMovingItem gameObject, Direction direction);
        PushStatus CanBePushedOrBounced(IMovingItem toBeMoved, IMovingItem byWhom, Direction direction, bool isBounceBackPossible);
        }
    }
