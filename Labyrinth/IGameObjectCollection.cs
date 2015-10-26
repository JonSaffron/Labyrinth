using System.Collections.Generic;
using Labyrinth.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public interface IGameObjectCollection
        {
        int Width { get; }
        int Height {get; }

        int CountOfShots { get; }

        void Remove(StaticItem gameObject);

        IEnumerable<MovingItem> InteractiveGameItems { get; }

        [CanBeNull]
        IEnumerable<StaticItem> ItemsAtPosition(TilePos tp);

        IEnumerable<StaticItem> DistinctItems();
        
        /// <summary>
        /// Adds a single item to the collection
        /// </summary>
        /// <param name="gameObject">Specifies the item to add</param>
        void Add(StaticItem gameObject);

        /// <summary>
        /// Allows the collection to update its knowledge of where the specified game object is located
        /// </summary>
        /// <param name="item">The game object whose position has changed</param>
        void UpdatePosition(MovingItem item);
        }
    }