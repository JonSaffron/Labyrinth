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
        private List<StaticItem>[] _allGameItems;
        private readonly LinkedList<MovingItem> _interactiveGameItems; 
        private static readonly IEnumerable<StaticItem> EmptyItemList = Enumerable.Empty<StaticItem>();
        private int _maxWidth;
        private int _maxHeight;

        /// <inheritdoc />
        public int CountOfShots { get; private set; }

        /// <summary>
        /// Initialises the collection
        /// </summary>
        public GameObjectCollection()
            {
            this._allGameItems = new List<StaticItem>[1];
            this._interactiveGameItems = new LinkedList<MovingItem>();
            }

        /// <inheritdoc />
        public void Add(StaticItem gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            InsertIntoAllGameItemsArray(gameObject);
            if (!(gameObject is MovingItem mi)) 
                return;

            mi.OriginalPosition = mi.Position;
            this._interactiveGameItems.AddLast(mi);
            if (mi is Shot)
                this.CountOfShots++;
            }

        /// <inheritdoc />
        public void Remove(StaticItem gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            if (gameObject is MovingItem movingItem)
                {
                bool wasItemRemoved = this._interactiveGameItems.Remove(movingItem);
                if (!wasItemRemoved)
                    throw new ArgumentOutOfRangeException(nameof(gameObject));
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
            EnsureArrayIsLargeEnough(gameObject.TilePosition);

            int mortonPosition = gameObject.TilePosition.MortonCode;
            List<StaticItem> listOfStaticItem = this._allGameItems[mortonPosition];
            if (listOfStaticItem == null)
                {
                listOfStaticItem = new List<StaticItem>();
                this._allGameItems[mortonPosition] = listOfStaticItem;
                }
            listOfStaticItem.Add(gameObject);
            }

        private void EnsureArrayIsLargeEnough(TilePos tp)
            {
            if (tp.X < 0 || tp.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(tp), "TilePosition cannot have an X or Y component that is less than zero.");
            if (tp.X <= this._maxWidth && tp.Y <= this._maxHeight)
                return;

            this._maxWidth = Math.Max(tp.X, this._maxWidth);
            this._maxHeight = Math.Max(tp.Y, this._maxHeight);
            var maxTilePos = new TilePos(this._maxWidth, this._maxHeight);
            int maxMortonCode = maxTilePos.MortonCode;
            Array.Resize(ref this._allGameItems, maxMortonCode + 1);
            }

        /// <inheritdoc />
        public IEnumerable<MovingItem> InteractiveGameItems
            {
            get
                {
                var result = new MovingItem[this._interactiveGameItems.Count];
                this._interactiveGameItems.CopyTo(result, 0);
                return result;
                }
            }

        /// <inheritdoc />
        public IEnumerable<StaticItem> ItemsAtPosition(TilePos tp)
            {
            if (tp.X < 0 || tp.Y < 0)
                return EmptyItemList;
            var mortonIndex = tp.MortonCode;
            if (mortonIndex >= this._allGameItems.GetLength(0))
                return EmptyItemList;
            var result = this._allGameItems[mortonIndex] ?? EmptyItemList;
            return result;
            }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void UpdatePosition(MovingItem item)
            {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            int previousMortonCode = TilePos.TilePosFromPosition(item.OriginalPosition).MortonCode;
            int newMortonCode = item.TilePosition.MortonCode;
            if (previousMortonCode == newMortonCode)
                return;

            ref var previousList = ref this._allGameItems[previousMortonCode];
            if (previousList == null)
                throw new InvalidOperationException();
            int countOfItems = previousList.Count;
            if (countOfItems == 0)
                throw new InvalidOperationException();
            if (countOfItems != 1)
                {
                previousList.Remove(item);
                }

            EnsureArrayIsLargeEnough(item.TilePosition);
            ref var newList = ref this._allGameItems[newMortonCode];
            if (newList == null && countOfItems == 1)
                {
                newList = previousList;
                }
            else if (newList == null)
                {
                newList = new List<StaticItem> { item };
                }
            else
                {
                newList.Add(item);
                }

            if (countOfItems == 1)
                {
                previousList = null;
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
