using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth.DataStructures
    {
    public class Grid
        {
        private readonly List<IGameObject>[,] _gameObjects;
        private static readonly IEnumerable<IGameObject> EmptyItemList = Enumerable.Empty<StaticItem>();
        private readonly TilePos _worldSize;

        public Grid(TilePos worldSize)
            {
            this._gameObjects = new List<IGameObject>[worldSize.X, worldSize.Y];
            this._worldSize = worldSize;
            }

        public void Add([NotNull] IGameObject gameObject)
            {
            if (gameObject == null) 
                throw new ArgumentNullException(nameof(gameObject));
            if (!gameObject.IsExtant)
                throw new ArgumentException("Cannot Add a non-extant object to the GameObjectCollection.");
            
            TilePos tp = gameObject.TilePosition;
            ref List<IGameObject> listOfStaticItem = ref this._gameObjects[tp.X, tp.Y];
            if (listOfStaticItem == null)
                {
                listOfStaticItem = new List<IGameObject>();
                }
            listOfStaticItem.Add(gameObject);
            }

        public void Remove([NotNull] IGameObject gameObject)
            {
            if (gameObject == null) 
                throw new ArgumentNullException(nameof(gameObject));

            TilePos tp = gameObject.TilePosition;
            List<IGameObject> listOfStaticItem = this._gameObjects[tp.X, tp.Y];
            if (listOfStaticItem == null || listOfStaticItem.Count == 0)
                throw new InvalidOperationException("The gameObjects collection where the gameObject was expected to be was null.");
            if (!listOfStaticItem.Remove(gameObject))
                throw new InvalidOperationException("The gameObject could not be found in the allGameItems list, or could not be removed.");
            }

        public IEnumerable<IGameObject> ItemsAtPosition(TilePos tp)
            {
            if (tp.X < 0 || tp.Y < 0 || tp.X >= this._worldSize.X || tp.Y >= this._worldSize.Y)
                return EmptyItemList;
            var result = this._gameObjects[tp.X, tp.Y] ?? EmptyItemList;
            return result;
            }

        /// <inheritdoc />
        public IEnumerable<IGameObject> DistinctItems()
            {
            var set = new HashSet<IGameObject>();
            for (int x = 0; x < this._worldSize.X; x++)
                {
                for (int y = 0; y < this._worldSize.Y; y++)
                    {
                    var list = this._gameObjects[x, y];
                    if (list == null)
                        continue;

                    foreach (var item in list)
                        {
                        if (set.Add(item))
                            yield return item;
                        }
                    }
                }
            }

        /// <inheritdoc />
        public void UpdatePosition(IMovingItem item)
            {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var originalTilePos = TilePos.TilePosFromPosition(item.OriginalPosition);
            var newTilePos = item.TilePosition;
            if (originalTilePos == newTilePos)
                return;

            ref var previousList = ref this._gameObjects[originalTilePos.X, originalTilePos.Y];
            if (previousList == null)
                throw new InvalidOperationException();
            int countOfItems = previousList.Count;
            if (countOfItems == 0)
                throw new InvalidOperationException();
            if (countOfItems != 1)
                {
                previousList.Remove(item);
                }

            ref var newList = ref this._gameObjects[newTilePos.X, newTilePos.Y];
            if (newList == null && countOfItems == 1)
                {
                newList = previousList;
                }
            else if (newList == null)
                {
                newList = new List<IGameObject> { item };
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

        public void FindItem(IMovingItem itemToFind)
            {
            System.Diagnostics.Trace.WriteLine($"Previous {TilePos.TilePosFromPosition(itemToFind.OriginalPosition)}    Actual {TilePos.TilePosFromPosition(itemToFind.Position)}");
            for (int x = 0; x < this._worldSize.X; x++)
                {
                for (int y = 0; y < this._worldSize.Y; y++)
                    {
                    var list = this._gameObjects[x, y];
                    if (list == null)
                        continue;

                    foreach (var item in list)
                        {
                        if (item == itemToFind)
                            System.Diagnostics.Trace.WriteLine($"{x},${y}");
                        }
                    }
                }
            }
        }
    }
