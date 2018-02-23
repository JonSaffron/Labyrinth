namespace Labyrinth
    {
    public interface IBoundMovementFactory
        {
        IBoundMovement GetExplicitBoundary();
        IBoundMovement GetBoundedInRoom();
        }
    }
