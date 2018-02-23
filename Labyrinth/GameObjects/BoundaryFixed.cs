namespace Labyrinth.GameObjects
    {
    class BoundaryFixed : IBoundMovement
        {
        private readonly TileRect _boundary;

        public BoundaryFixed(TileRect boundary)
            {
            this._boundary = boundary;
            }

        public static BoundaryFixed FromSize(TilePos size)
            {
            return new BoundaryFixed(new TileRect(new TilePos(0, 0), size.X, size.Y));
            }

        public bool IsPositionWithinMovementBoundaries(TilePos tilePos)
            {
            var result = this._boundary.Contains(tilePos);
            return result;
            }
        }
    }
