using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    class StandardPatrolling : MonsterMotionBase
        {
        private Direction _currentDirection;

        public StandardPatrolling([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._currentDirection = initialDirection;
            }

        public override Direction DetermineDirection()
            {
            if (this.Monster.CanMoveInDirection(this._currentDirection))
                return this._currentDirection;
            var reversed = this._currentDirection.Reversed();
            if (this.Monster.CanMoveInDirection(reversed))
                return reversed;
            return Direction.None;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            this._currentDirection = direction;
            return true;
            }
        }
    }
