using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    internal class BoundaryFixed : IBoundMovement
        {
        private readonly TileRect _boundary;

        public BoundaryFixed(TileRect boundary)
            {
            this._boundary = boundary;
            }

        public bool IsPositionWithinBoundary(TilePos tilePos)
            {
            var result = this._boundary.Contains(tilePos);
            return result;
            }
        }
    }
