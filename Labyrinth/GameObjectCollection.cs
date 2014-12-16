using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class GameObjectCollection
        {
        private readonly LinkedList<StaticItem> _allGameItems; 
        private readonly LinkedList<StaticItem> _interactiveGameItems; 

        public GameObjectCollection(IEnumerable<StaticItem> initialItems = null)
            {
            this._allGameItems = new LinkedList<StaticItem>();
            this._interactiveGameItems = new LinkedList<StaticItem>();
            if (initialItems != null)
                this.AddRange(initialItems);
            }

        public void AddRange(IEnumerable<StaticItem> objects)
            {
            if (objects == null)
                throw new ArgumentNullException("objects");

            foreach (var item in objects)
                Add(item);
            }

        public void Add(StaticItem gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");

            InsertIntoList(this._allGameItems, gameObject);
            if (!(gameObject is Wall))
                InsertIntoList(this._interactiveGameItems, gameObject);
            }

        private static void InsertIntoList(LinkedList<StaticItem> list, StaticItem gameObject)
            {
            int zOrder = GetZOrder(gameObject);
            var listItem = list.First;
            while (listItem != null)
                {
                int listItemZOrder = GetZOrder(listItem.Value);
                if (listItemZOrder > zOrder)
                    {
                    list.AddBefore(listItem, gameObject);
                    return;
                    }

                listItem = listItem.Next;
                }
            list.AddLast(gameObject);
            }

        public IEnumerable<StaticItem> AllItemsInRectangle(Rectangle r)
            {
            var result = this._allGameItems.Where(item => r.Contains((int) item.Position.X, (int) item.Position.Y));
            return result;
            }

        public IEnumerable<StaticItem> GetSurvivingInteractiveItems()
            {
            var item = this._interactiveGameItems.First;
            while (item != null)
                {
                var currentNode = item;
                var gameObject = item.Value;
                item = item.Next;

                if (!gameObject.IsExtant && !(gameObject is Player))
                    {
                    this._interactiveGameItems.Remove(currentNode);
                    continue;
                    }

                yield return gameObject;
                }
            }

        public IEnumerable<StaticItem> AllItems
            {
            get
                {
                var result = new StaticItem[this._allGameItems.Count];
                this._allGameItems.CopyTo(result, 0);
                return result;
                }
            }

        public IEnumerable<StaticItem> InteractiveItems
            {
            get
                {
                var result = new StaticItem[this._interactiveGameItems.Count];
                this._interactiveGameItems.CopyTo(result, 0);
                return result;
                }
            }

        public IEnumerable<StaticItem> GetItemsOnTile(TilePos tp)
            {
            var result = this._allGameItems.Where(gi => gi.IsExtant && gi.TilePosition == tp);
            return result;
            } 

        public void RemoveBangsAndShots()
            {
            var item = this._interactiveGameItems.First;
            while (item != null)
                {
                var currentNode = item;
                item = item.Next;

                if (currentNode.Value is Bang || currentNode.Value is Shot)
                    {
                    currentNode.Value.InstantlyExpire();

                    //this._allGameItems.Remove(currentNode.Value);
                    //this._interactiveGameItems.Remove(currentNode);
                    }
                }
            }

        private static int GetZOrder(StaticItem si)
            {
            if (si == null)
                throw new ArgumentNullException("si");

            if (si is Wall)
                return 0;
            if (!(si is MovingItem) && !(si is Bang))
                return 1;
            var monster = si as Monster.Monster;
            if (monster != null)
                {
                var result = monster.IsStill ? 2 : 3;
                return result;
                }
            if (si is Player)
                return 4;
            if (si is Boulder)
                return 5;
            if (si is Shot)
                return 6;
            if (si is Bang)
                return 7;

            throw new InvalidOperationException();
            }
        }
    }
