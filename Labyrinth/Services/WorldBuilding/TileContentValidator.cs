using System.Collections.Generic;
using System.Linq;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.WorldBuilding
    {
    class TileContentValidator
        {
        public bool IsListOfObjectsValid(IEnumerable<StaticItem> objects, out string reason)
            {
            var list = new List<StaticItem>(objects ?? Enumerable.Empty<StaticItem>());

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
