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
                    reason = "Multiple impassable objects.";
                    return false;
                    }
                }

            var countOfMoveableObjects = list.Count(item => item.Solidity == ObjectSolidity.Moveable);
            if (countOfMoveableObjects > 1)
                {
                reason = "Multiple moveable objects";
                return false;
                }

            var countOfStationaryObjects = list.Count(item => item.Solidity == ObjectSolidity.Stationary);
            if (countOfStationaryObjects > 1)
                {
                reason = "Multiple stationary objects";
                return false;
                }

            if (countOfMoveableObjects != 0 && list.Any(item => item.Solidity == ObjectSolidity.Insubstantial))
                {
                reason = "A moveable object and an insubstantial object.";
                return false;
                }

            reason = string.Empty;
            return true;
            }
        }
    }
