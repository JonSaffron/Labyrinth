namespace Labyrinth
    {
    public interface IBoundMovementFactory
        {
        IBoundMovement GetWorldBoundary();
        IBoundMovement GetExplicitBoundary(TileRect boundary);
        IBoundMovement GetBoundedInRoom(TilePos tilePos);
        }
    }
