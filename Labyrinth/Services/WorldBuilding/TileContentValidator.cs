using System.Collections.Generic;
using System.Linq;
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
        public bool IsListOfObjectsValid(IEnumerable<IGameObject> objects, out string reason)
            {
            var list = new List<IGameObject>(objects ?? Enumerable.Empty<StaticItem>());

            if (list.Count == 0)
                {
                reason = string.Empty;
                return true;
                }

            if (list.Any(item => item.Solidity == ObjectSolidity.Impassable))
                {
                var result = list.Count == 1;
                if (!result)
                    {
                    reason = "Multiple impassable objects: " + string.Join(", ", list.Where(item => item.Solidity == ObjectSolidity.Impassable).Select(item => item.GetType().Name));
                    return false;
                    }
                }

            var countOfMoveableObjects = list.Count(item => item.Solidity == ObjectSolidity.Moveable);
            if (countOfMoveableObjects > 1)
                {
                reason = "Multiple moveable objects: " + string.Join(", ", list.Where(item => item.Solidity == ObjectSolidity.Moveable).Select(item => item.GetType().Name));
                return false;
                }

            var countOfStationaryObjects = list.Count(item => item.Solidity == ObjectSolidity.Stationary);
            if (countOfStationaryObjects > 1)
                {
                reason = "Multiple stationary objects: " + string.Join(", ", list.Where(item => item.Solidity == ObjectSolidity.Stationary).Select(item => item.GetType().Name));
                return false;
                }

            if (countOfMoveableObjects != 0 && list.Any(item => item.Solidity == ObjectSolidity.Insubstantial))
                {
                reason = "A moveable object and an insubstantial object: " + list.Single(item => item.Solidity == ObjectSolidity.Moveable).GetType().Name + ", " + list.Where(item => item.Solidity == ObjectSolidity.Insubstantial).Select(item => item.GetType().Name);
                return false;
                }

            reason = string.Empty;
            return true;
            }
        }
    }
