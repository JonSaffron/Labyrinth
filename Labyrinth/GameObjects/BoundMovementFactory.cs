namespace Labyrinth.GameObjects
    {
    public class BoundMovementFactory : IBoundMovementFactory
        {
        private readonly TileRect _worldBoundary;

        public BoundMovementFactory(TileRect worldBoundary)
            {
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
