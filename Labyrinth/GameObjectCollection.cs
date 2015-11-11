using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    /// <summary>
    /// Keeps track of the objects in the game
    /// </summary>
    public class GameObjectCollection : IGameObjectCollection
        {
        private readonly List<StaticItem>[] _allGameItems;
        private readonly LinkedList<MovingItem> _interactiveGameItems; 

        private readonly int _width;
        private readonly int _height;

        public int CountOfShots { get; private set; }

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

            this._width = width;
            this._height = height;
            var maxTilePosition = new TilePos(width, height);
            this._allGameItems = new List<StaticItem>[maxTilePosition.MortonCode];
            this._interactiveGameItems = new LinkedList<MovingItem>();
            if (initialItems != null)
                this.AddRange(initialItems);
            }

        public int Width
            {
            get
                {
                return this._width;
                }
            }

        public int Height
            {
            get
                {
                return this._height;
                }
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
                mi.OriginalPosition = mi.Position;
                this._interactiveGameItems.AddLast(mi);
                if (mi is Shot)
                    this.CountOfShots++;
                }
            }

        public void Remove(StaticItem gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");

            var movingItem = gameObject as MovingItem;
            if (movingItem != null)
                {
                bool wasItemRemoved = this._interactiveGameItems.Remove(movingItem);
                if (!wasItemRemoved)
                    throw new ArgumentOutOfRangeException("gameObject");
                }

            if (gameObject is Shot)
                this.CountOfShots--;

            int mortonPosition = gameObject.TilePosition.MortonCode;
            List<StaticItem> listOfStaticItem = this._allGameItems[mortonPosition];
            if (listOfStaticItem == null || listOfStaticItem.Count == 0)
                throw new InvalidOperationException("The allGameItems collection where the gameObject was expected to be was null.");
            if (!listOfStaticItem.Remove(gameObject))
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


        public IEnumerable<MovingItem> InteractiveGameItems
            {
            get
                {
                var result = new MovingItem[this._interactiveGameItems.Count];
                this._interactiveGameItems.CopyTo(result, 0);
                return result;
                }
            }

        public IEnumerable<StaticItem> ItemsAtPosition(TilePos tp)
            {
            if (tp.X < 0 || tp.Y < 0 || tp.X >= this.Width || tp.Y >= this.Height)
                throw new ArgumentOutOfRangeException("tp");
            var mortonIndex = tp.MortonCode;
            var result = this._allGameItems[mortonIndex] ?? Enumerable.Empty<StaticItem>();
            return result;
            }

        public IEnumerable<StaticItem> DistinctItems()
            {
            var set = new HashSet<StaticItem>();
            // ReSharper disable once LoopCanBeConvertedToQuery (leads to slow delegate)
            foreach (var list in this._allGameItems.WhereNotNull())
                {
                foreach (var item in list)
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

            item.OriginalPosition = item.Position;
            }

        public void FindItem(MovingItem itemToFind)
            {
            System.Diagnostics.Trace.WriteLine(string.Format("Previous {0}    Actual {1}", TilePos.TilePosFromPosition(itemToFind.OriginalPosition), TilePos.TilePosFromPosition(itemToFind.Position)));
            int max = this._allGameItems.GetLength(0);
            for (int i = 0; i < max; i++)
                {
                var list = this._allGameItems[i];
                if (list == null)
                    continue;

                foreach (var item in list)
                    {
                    if (item == itemToFind)
                        System.Diagnostics.Trace.WriteLine(string.Format("{0} {1}", i, TilePos.FromMortonCode(i)));
                    }
                }
            }
        }
    }
