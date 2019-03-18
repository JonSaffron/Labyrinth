using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IGameObjectCollection
        {
        /// <summary>
        /// Adds a single item to the collection
        /// </summary>
        /// <param name="gameObject">Specifies the item to add</param>
        void Add([NotNull] IGameObject gameObject);

        /// <summary>
        /// Removes a single item from the collection
        /// </summary>
        /// <param name="gameObject">Specified the item to remove</param>
        void Remove([NotNull] IGameObject gameObject);

        /// <summary>
        /// Allows the collection to update its knowledge of where the specified game object is located
        /// </summary>
        /// <param name="item">The game object whose position has changed</param>
        void UpdatePosition(IMovingItem item);

        /// <summary>
        /// Returns a list of all the moving items in the world
        /// </summary>
        /// <remarks>Used for the update loop</remarks>
        IEnumerable<IMovingItem> InteractiveGameItems { get; }

        /// <summary>
        /// Returns a list of all the items at the specified position
        /// </summary>
        /// <param name="tp">Specifies the position to return the occupants at</param>
        /// <returns>A list of the occupants at the specified position, or an empty list otherwise</returns>
        /// <remarks>Used to determine if movement is possible, or a new object can occupy the position</remarks>
        IEnumerable<IGameObject> ItemsAtPosition(TilePos tp);

        /// <summary>
        /// Returns a list of all items in the world
        /// </summary>
        /// <returns>A distinct list of all the items in the world, even when an object spans more than one tile</returns>
        /// <remarks>Generally used to get objects of a given type</remarks>
        IEnumerable<IGameObject> DistinctItems();

        /// <summary>
        /// Returns the current number of shot objects in the world
        /// </summary>
        int CountOfShots { get; }
        }
    }