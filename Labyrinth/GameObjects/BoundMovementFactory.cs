namespace Labyrinth.GameObjects
    {
    public class BoundMovementFactory : IBoundMovementFactory
        {
        private readonly TileRect _worldBoundary;

        public BoundMovementFactory(TilePos worldSize)
            {
            var worldBoundary = new TileRect(new TilePos(0, 0), worldSize.X, worldSize.Y);
            this._worldBoundary = worldBoundary;
            }

        public IBoundMovement GetWorldBoundary()
            {
            return new BoundaryFixed(this._worldBoundary);
            }

        public IBoundMovement GetExplicitBoundary(TileRect boundary)
            {
            return new BoundaryFixed(boundary);
            }

        public IBoundMovement GetBoundedInRoom(TilePos tilePos)
            {
            var boundary = World.GetContainingRoom(tilePos);
            return new BoundaryFixed(boundary);
            }
        }
    }
