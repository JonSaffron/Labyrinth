using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.WorldBuilding
    {
    class TileContentValidator
        {
        /// <summary>
        /// Checks whether the supplied list of game objects are able to share the same tile
        /// </summary>
        /// <param name="objects">A list of objects to check</param>
        /// <param name="reason">Returns a reason why the objects cannot share the same tile, or an empty string if there are no problems found.</param>
        /// <returns>True if the specified objects can share the same tile, or false otherwise.</returns>
        public bool IsListOfObjectsValid([CanBeNull] IEnumerable<IGameObject> objects, out string reason)
            {
            var list = new List<IGameObject>(objects ?? Enumerable.Empty<StaticItem>());

            if (list.Count == 0)
                {
                reason = string.Empty;
                return true;
                }

            if (list.Any(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Impassable))
                {
                var isThereOneAndOnlyOneObjectInTheList = list.Count == 1;
                if (!isThereOneAndOnlyOneObjectInTheList)
                    {
                    reason = "Multiple impassable objects: " + ObjectListToString(list.Where(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Impassable));
                    return false;
                    }
                }

            var countOfMoveableObjects = list.Count(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Moveable);
            if (countOfMoveableObjects > 1)
                {
                reason = "Multiple moveable objects: " + ObjectListToString(list.Where(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Moveable));
                return false;
                }

            var countOfStationaryObjects = list.Count(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Stationary);
            if (countOfStationaryObjects > 1)
                {
                reason = "Multiple stationary objects: " + ObjectListToString(list.Where(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Stationary));
                return false;
                }

            if (countOfMoveableObjects != 0 && list.Any(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Insubstantial))
                {
                reason = "A moveable object and an insubstantial object: " + list.Single(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Moveable).GetType().Name 
                                                                           + " and " + ObjectListToString(list.Where(item => item.Properties.Get(GameObjectProperties.Solidity) == ObjectSolidity.Insubstantial));
                return false;
                }

            reason = string.Empty;
            return true;
            }

        private static string ObjectListToString(IEnumerable<IGameObject> items)
            {
            IEnumerable<string> itemsAsStrings = items.Select(item => item.GetType().Name);
            string result = string.Join("+", itemsAsStrings);
            return result;
            }
        }
    }
