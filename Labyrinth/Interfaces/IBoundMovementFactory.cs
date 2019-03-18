using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IBoundMovementFactory
        {
        /// <summary>
        /// Gets a boundary that encompasses the entire world area
        /// </summary>
        /// <returns>An object that implements IBoundMovement.</returns>
        IBoundMovement GetWorldBoundary();

        /// <summary>
        /// Gets a boundary for the specified area
        /// </summary>
        /// <param name="boundary">The boundary to apply</param>
        /// <returns>An object that implements IBoundMovement.</returns>
        IBoundMovement GetExplicitBoundary(TileRect boundary);

        /// <summary>
        /// Gets a boundary that encompasses the room that the specified GameObject is currently in
        /// </summary>
        /// <param name="gameObject">The GameObject to use to derive the boundary from</param>
        /// <returns>An object that implements IBoundMovement.</returns>
        IBoundMovement GetBoundedInRoom(IMovingItem gameObject);
        }
    }
