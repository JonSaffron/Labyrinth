using System;
using System.Collections.Generic;

namespace Labyrinth.GameObjects.Movement
    {
    class PatrolPerimeter : IMonsterMovement
        {
        private Direction _lastDirection;

        public PatrolPerimeter(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._lastDirection = initialDirection;
            }

        public Direction DetermineDirection(Monster monster)
            {
            if (TryDetermineDirection(monster, this._lastDirection, out Direction newDirection))
                {
                this._lastDirection = newDirection;
                return newDirection;
                }
            return Direction.None;
            }

        private static bool TryDetermineDirection(Monster monster, Direction currentDirection, out Direction newDirection)
            {
            using (var directions = GetPreferredDirections(currentDirection).GetEnumerator())
                {
                directions.MoveNext();
                newDirection = directions.Current;

                var canGoInNewDirection = monster.CanMoveInDirection(newDirection);
                if (!canGoInNewDirection)
                    {
                    // so there's definitely one or more walls. Just need to get the first direction we can go in.
                    while (directions.MoveNext())
                        {
                        newDirection = directions.Current;
                        if (monster.CanMoveInDirection(newDirection))
                            {
                            return true;
                            }
                        }
                    // can't go in any direction
                    newDirection = Direction.None;
                    return false;
                    }

return true;
                // check if there are any walls as we don't want to end up travelling in a small circle
                while (directions.MoveNext())
                    {
                    if (!monster.CanMoveInDirection(directions.Current))
                        {
                        return true;
                        }
                    }

                // monster could go in any direction as it's not next to any wall - continue current direction
                newDirection = currentDirection;
                return true;
                }
            }

        public static IEnumerable<Direction> GetPreferredDirections(Direction direction)
            {
            var start = Array.IndexOf(Clockwise, direction);
            for (int i = 0; i < 4; i++)
                {
                var elementIndex = (start + 5 - i) % 4;
                yield return Clockwise[elementIndex];
                }
            }

        private static readonly Direction[] Clockwise = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
        }
    }
