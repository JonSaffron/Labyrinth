using JetBrains.Annotations;

namespace Labyrinth
    {
    public interface IMovementChecker
        {
        bool CanMoveForwards([NotNull] IMovingItem source, Direction direction);
        bool CanBePushedBackDueToBounceBack([NotNull] IMovingItem gameObject, Direction direction);
        PushStatus CanBePushedOrBounced([NotNull] IMovingItem toBeMoved, [NotNull] IMovingItem byWhom, Direction direction, bool isBounceBackPossible);
        }
    }
