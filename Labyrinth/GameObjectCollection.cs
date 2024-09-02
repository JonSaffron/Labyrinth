using System;
using System.Collections.Generic;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    /// <summary>
    /// Keeps track of the objects in the game
    /// </summary>
    public class GameObjectCollection : IGameObjectCollection
        {
        private readonly Grid _grid;
        private readonly SimpleList<IGameObject> _allGameObjects;

        /// <inheritdoc />
        public int CountOfShots { get; private set; }

        /// <summary>
        /// Initialises the collection
        /// </summary>
        public GameObjectCollection(TilePos worldSize)
            {
            this._grid = new Grid(worldSize);
            this._allGameObjects = new SimpleList<IGameObject>(400);
            }

        /// <inheritdoc />
        public void Add(IGameObject gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (!gameObject.IsExtant)
                throw new ArgumentException("Cannot Add a non-extant object to the GameObjectCollection.");

            this._grid.Add(gameObject);
            this._allGameObjects.Add(gameObject);
            if (!(gameObject is IMovingItem mi)) 
                return;

            mi.OriginalPosition = mi.Position;
            if (mi is IStandardShot)
                this.CountOfShots++;
            }

        /// <inheritdoc />
        public void Remove(IGameObject gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            if (gameObject is Player)
                throw new ArgumentOutOfRangeException(nameof(gameObject), "Cannot remove Player object from collection.");

            this._grid.Remove(gameObject);
            var indexOfItem = this._allGameObjects.IndexOf(gameObject);
            if (indexOfItem == -1)
                throw new ArgumentOutOfRangeException(nameof(gameObject));
            this._allGameObjects.RemoveAt(indexOfItem);

            if (gameObject is IStandardShot)
                this.CountOfShots--;
            }

        /// <inheritdoc />
        public IEnumerable<IGameObject> AllGameObjects
            {
            get
                {
                var result = new IGameObject[this._allGameObjects.Length];
                this._allGameObjects.CopyTo(result);
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
