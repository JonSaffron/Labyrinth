using System;
using System.Collections.Generic;
using Labyrinth.DataStructures;

namespace Labyrinth
    {
    /// <summary>
    /// Keeps track of the objects in the game
    /// </summary>
    public class GameObjectCollection : IGameObjectCollection
        {
        private readonly Grid _grid;
        private readonly SimpleList<IMovingItem> _interactiveGameItems; 

        /// <inheritdoc />
        public int CountOfShots { get; private set; }

        /// <summary>
        /// Initialises the collection
        /// </summary>
        public GameObjectCollection(TilePos worldSize)
            {
            this._grid = new Grid(worldSize);
            this._interactiveGameItems = new SimpleList<IMovingItem>(100);
            }

        /// <inheritdoc />
        public void Add(IGameObject gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (!gameObject.IsExtant)
                throw new ArgumentException("Cannot Add a non-extant object to the GameObjectCollection.");

            this._grid.Add(gameObject);
            if (!(gameObject is IMovingItem mi)) 
                return;

            mi.OriginalPosition = mi.Position;
            this._interactiveGameItems.Add(mi);
            if (mi is IStandardShot)
                this.CountOfShots++;
            }

        /// <inheritdoc />
        public void Remove(IGameObject gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            if (gameObject is IMovingItem movingItem)
                {
                var indexOfItem = this._interactiveGameItems.IndexOf(movingItem);
                if (indexOfItem == -1)
                    throw new ArgumentOutOfRangeException(nameof(gameObject));
                
                this._interactiveGameItems.RemoveAt(indexOfItem);
                }

            if (gameObject is IStandardShot)
                this.CountOfShots--;

            this._grid.Remove(gameObject);
            }

        /// <inheritdoc />
        public IEnumerable<IMovingItem> InteractiveGameItems
            {
            get
                {
                var result = new IMovingItem[this._interactiveGameItems.Length];
                this._interactiveGameItems.CopyTo(result);
                return result;
                }
            }

        /// <inheritdoc />
        public IEnumerable<IGameObject> ItemsAtPosition(TilePos tp)
            {
            return this._grid.ItemsAtPosition(tp);
            }

        /// <inheritdoc />
        public IEnumerable<IGameObject> DistinctItems()
            {
            return this._grid.DistinctItems();
            }

        /// <inheritdoc />
        public void UpdatePosition(IMovingItem item)
            {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this._grid.UpdatePosition(item);
            }
        }
    }
