namespace Labyrinth.GameObjects
    {
    public class BoundMovementFactory : IBoundMovementFactory
        {
        private readonly BoundaryFixed _worldBoundary;

        public BoundMovementFactory(TilePos worldSize)
            {
            var worldBoundary = new TileRect(TilePos.Zero, worldSize.X, worldSize.Y);
            this._worldBoundary = new BoundaryFixed(worldBoundary);
            }

        public IBoundMovement GetWorldBoundary()
            {
            return this._worldBoundary;
            }

        public IBoundMovement GetExplicitBoundary(TileRect boundary)
            {
            return new BoundaryFixed(boundary);
            }

        public IBoundMovement GetBoundedInRoom(IMovingItem gameObject)
            {
            return new BoundaryCurrentRoom(gameObject);
            }
        }
    }
