using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    class PatrolPerimeter : MonsterMotionBase
        {
        private Direction _lastDirection;
        private bool _turnType;
        private bool _isDetached;

        private static readonly Direction[] Clockwise = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
        private static readonly Direction[] Anticlockwise = { Direction.Left, Direction.Down, Direction.Right, Direction.Up };

        public PatrolPerimeter([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._lastDirection = initialDirection;
            }

        public override Direction DetermineDirection()
            {
            Direction result = !this._isDetached
                ? DetermineDirectionWhenAttached()
                : DetermineDirectionWhenDetached();
            return result;
            }

        private Direction DetermineDirectionWhenAttached()
            { 
            using (var directions = GetPreferredDirections(this._lastDirection, this._turnType).GetEnumerator())
                {
                directions.MoveNext();
                var newDirection = directions.Current;

                var canGoInNewDirection = this.Monster.CanMoveInDirection(newDirection);
                if (!canGoInNewDirection)
                    {
                    // so there's definitely one or more walls. Just need to get the first direction we can go in.
                    while (directions.MoveNext())
                        {
                        newDirection = directions.Current;
                        if (this.Monster.CanMoveInDirection(newDirection))
                            {
                            return newDirection;
                            }
                        }
                    // can't go in any direction
                    return Direction.None;
                    }

                // check if there are any walls as we don't want to end up travelling in a small circle
                while (directions.MoveNext())
                    {
                    if (!this.Monster.CanMoveInDirection(directions.Current))
                        {
                        return newDirection;
                        }
                    }

                // monster could go in any direction as it's not next to any wall - continue current direction
                this._isDetached = true;
                return this._lastDirection;
                }
            }

        private Direction DetermineDirectionWhenDetached()
            {
            if (this.Monster.CanMoveInDirection(this._lastDirection))
                {
                return this._lastDirection;
                }

            // Aha. We have bumped into something so we need to re-attach.
            using (var directions = GetPreferredDirections(this._lastDirection, this._turnType).GetEnumerator())
                {
                while (directions.MoveNext())
                    {
                    var newDirection = directions.Current;
                    if (this.Monster.CanMoveInDirection(newDirection))
                        {
                        // Upon attaching, we need to reverse the rotation that the monster uses to plan its next move
                        this._turnType = !this._turnType;
                        this._isDetached = false;
                        return newDirection;
                        }
                    }
                }

            // can't go in any direction
            return Direction.None;
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

       public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.StandardSpeed);
            this._lastDirection = direction;
            return true;
            }
        }
    }
