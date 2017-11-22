using System;
using System.Collections.Generic;

namespace Labyrinth.GameObjects.Movement
    {
    // todo
    class PatrolPerimeter : IMonsterMovement
        {
        private Direction _lastDirection;
        private bool _turnType;
        private bool _isDetached;

        public PatrolPerimeter(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._lastDirection = initialDirection;
            }

        public Direction DetermineDirection(Monster monster)
            {
            Direction newDirection;
            bool willMove = !this._isDetached 
                            ? TryDetermineDirection(monster, this._lastDirection, out newDirection) 
                            : TryToDetermineDirectionWhenDetached(monster, this._lastDirection, out newDirection);
            if (willMove)
                {
                this._lastDirection = newDirection;
                return newDirection;
                }
            return Direction.None;
            }

        private bool TryDetermineDirection(Monster monster, Direction currentDirection, out Direction newDirection)
            {
            using (var directions = GetPreferredDirections(currentDirection, this._turnType).GetEnumerator())
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
                this._isDetached = true;
                return true;
                }
            }

        public static IEnumerable<Direction> GetPreferredDirections(Direction direction, bool turnDirection)
            {
            var dirs = turnDirection ? Clockwise : Anticlockwise;
            var start = Array.IndexOf(dirs, direction);
            for (int i = 0; i < 4; i++)
                {
                var elementIndex = (start + 5 - i) % 4;
                yield return dirs[elementIndex];
                }
            }

        private bool TryToDetermineDirectionWhenDetached(Monster monster, Direction currentDirection, out Direction newDirection)
            {
            if (monster.CanMoveInDirection(currentDirection))
                {
                newDirection = currentDirection;
                return true;
                }

            using (var directions = GetPreferredDirections(currentDirection, this._turnType).GetEnumerator())
                {
                while (directions.MoveNext())
                    {
                    newDirection = directions.Current;
                    if (monster.CanMoveInDirection(newDirection))
                        {
                        this._turnType = !this._turnType;
                        this._isDetached = false;
                        return true;
                        }
                    }
                }

            // can't go in any direction
            newDirection = Direction.None;
            return false;
            }

        private static readonly Direction[] Clockwise = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
        private static readonly Direction[] Anticlockwise = { Direction.Left, Direction.Down, Direction.Right, Direction.Up };

        }
    }
