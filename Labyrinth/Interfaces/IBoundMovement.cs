using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IBoundMovement
        {
        bool IsPositionWithinBoundary(TilePos tilePos);
        }
    }
