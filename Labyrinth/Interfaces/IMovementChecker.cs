using JetBrains.Annotations;

namespace Labyrinth
    {
    public interface IMovementChecker
        {
        bool CanMove([NotNull] IMovingItem source, Direction direction);
        void PushOrBounce([NotNull] IMovingItem initiatingObject, [NotNull] IMovingItem moveableObject, Direction direction);
        }
    }
