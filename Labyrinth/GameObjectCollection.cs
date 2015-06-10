using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    /// <summary>
    /// Keeps track of the objects in the game
    /// </summary>
    class GameObjectCollection
        {
        private readonly List<StaticItem>[] _allGameItems;
        private readonly LinkedList<MovingItem> _interactiveGameItems; 
        private int _countOfShots;

        /// <summary>
        /// Initialises the collection
        /// </summary>
        /// <param name="width">The width of the game world in tiles</param>
        /// <param name="height">The height of the game world in tiles</param>
        /// <param name="initialItems">An optional initial list of items to add to the collection</param>
        public GameObjectCollection(int width, int height, IEnumerable<StaticItem> initialItems = null)
            {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            var maxTilePosition = new TilePos(width, height);
            this._allGameItems = new List<StaticItem>[maxTilePosition.MortonCode];
            this._interactiveGameItems = new LinkedList<MovingItem>();
            if (initialItems != null)
                this.AddRange(initialItems);
            }

        private void AddRange(IEnumerable<StaticItem> objects)
            {
            if (objects == null)
                throw new ArgumentNullException("objects");
                
            foreach (var item in objects)
                Add(item);
            }

        /// <summary>
        /// Adds a single item to the collection
        /// </summary>
        /// <param name="gameObject">Specifies the item to add</param>
        public void Add(StaticItem gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");

            InsertIntoAllGameItemsArray(gameObject);
            var mi = gameObject as MovingItem;
            if (mi != null)
                {
                this._interactiveGameItems.AddLast(mi);
                if (mi is Shot)
                    this._countOfShots++;
                }
            }

        private void Remove(LinkedListNode<MovingItem> gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");

            if (gameObject.Value is Shot)
                this._countOfShots--;
            this._interactiveGameItems.Remove(gameObject);

            int mortonPosition = gameObject.Value.TilePosition.MortonCode;
            List<StaticItem> listOfStaticItem = this._allGameItems[mortonPosition];
            if (listOfStaticItem == null || listOfStaticItem.Count == 0)
                throw new InvalidOperationException("The allGameItems collection where the gameObject was expected to be was null.");
            if (!listOfStaticItem.Remove(gameObject.Value))
                throw new InvalidOperationException("The gameObject could not be found in the allGameItems list, or could not be removed.");
            }

        private void InsertIntoAllGameItemsArray(StaticItem gameObject)
            {
            int mortonPosition = gameObject.TilePosition.MortonCode;
            List<StaticItem> listOfStaticItem = this._allGameItems[mortonPosition];
            if (listOfStaticItem == null)
                {
                listOfStaticItem = new List<StaticItem>();
                this._allGameItems[mortonPosition] = listOfStaticItem;
                }
            listOfStaticItem.Add(gameObject);
            }

        /// <summary>
        /// Retrieves all game objects within the specified rectangle
        /// </summary>
        /// <param name="r">The area to return items for</param>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<StaticItem> AllItemsInRectangle(Rectangle r)
            {
            var start = TilePos.TilePosFromPosition(new Vector2(r.X, r.Y)).MortonCode;
            var end = TilePos.TilePosFromPosition(new Vector2(r.Right, r.Bottom)).MortonCode;
            for (int i = start; i <= end; i++)
                {
                // for performance reasons we get the each item in turn from the array to allow for CPU caching
                if (i >= this._allGameItems.GetLength(0))
                    continue;
                var listOfItems = this._allGameItems[i];
                if (listOfItems == null)
                    continue;
                var tp = TilePos.FromMortonCode(i);
                var rect = tp.ToRectangle();
                if (!r.Intersects(rect))
                    continue;

                var copyOfList = listOfItems.ToArray();
                foreach (var item in copyOfList)
                    yield return item;
                }
            }

        /// <summary>
        /// Returns a list of all extant game objects that could interact with other game objects
        /// </summary>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<MovingItem> GetSurvivingInteractiveItems()
            {
            var item = this._interactiveGameItems.First;
            while (item != null)
                {
                var currentNode = item;
                var gameObject = item.Value;
                item = item.Next;

                if (!gameObject.IsExtant && !(gameObject is Player))
                    {
                    this.Remove(currentNode);
                    continue;
                    }

                yield return gameObject;
                }
            }

        /// <summary>
        /// Returns a list of all game objects of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<T> DistinctItemsOfType<T>() where T: StaticItem
            {
            var set = new HashSet<T>();
            // ReSharper disable once LoopCanBeConvertedToQuery (leads to slow delegate)
            foreach (var list in this._allGameItems.WhereNotNull())
                {
                foreach (var item in list.OfType<T>())
                    {
                    if (set.Add(item))
                        yield return item;
                    }
                }
            }

        /// <summary>
        /// Allows the collection to update its knowledge of where the specified game object is located
        /// </summary>
        /// <param name="item">The game object whose position has changed</param>
        public void UpdatePosition(MovingItem item)
            {
            if (item == null)
                throw new ArgumentNullException("item");

            int previousMortonCode = TilePos.TilePosFromPosition(item.OriginalPosition).MortonCode;
            int newMortonCode = item.TilePosition.MortonCode;
            if (previousMortonCode == newMortonCode)
                return;

            var previousList = this._allGameItems[previousMortonCode];
            if (previousList == null)
                throw new InvalidOperationException();
            int countOfItems = previousList.Count;
            if (countOfItems == 0)
                throw new InvalidOperationException();
            if (countOfItems == 1)
                {
                this._allGameItems[previousMortonCode] = null;
                }
            else
                {
                previousList.Remove(item);
                }
            
            var newList = this._allGameItems[newMortonCode];
            if (newList == null && countOfItems == 1)
                {
                this._allGameItems[newMortonCode] = previousList;
                }
            else if (newList == null)
                {
                this._allGameItems[newMortonCode] = new List<StaticItem> { item };
                }
            else
                {
                newList.Add(item);
                }
            }

        /// <summary>
        /// Returns a list of all extant game objects that are located on the specified tile
        /// </summary>
        /// <param name="tp">Specifes the tile position to inspect</param>
        /// <returns>A lazy enumeration of all the matching game objects</returns>
        public IEnumerable<StaticItem> GetItemsOnTile(TilePos tp)
            {
            var mortonPosition = tp.MortonCode;
            var listOfItems = this._allGameItems[mortonPosition] ?? new List<StaticItem>();
            var result = listOfItems.Where(gi => gi.IsExtant && gi.TilePosition == tp);
            return result;
            } 

        /// <summary>
        /// Removes all bangs and shots from the game object collection
        /// </summary>
        public void RemoveBangsAndShots()
            {
            foreach (var list in this._allGameItems.WhereNotNull())
                {
                foreach (var item in list)
                    {
                    if (item is Bang || item is Shot)
                        {
                        item.InstantlyExpire();
                        }
                    }
                }
            }

        /// <summary>
        /// Returns whether there are any shots currently in the collection
        /// </summary>
        /// <returns>True if a shot exits in the collection, otherwise False.</returns>
        public bool DoesShotExist()
            {
            var result = (this._countOfShots > 0);
            return result;
            }
        }
    }
