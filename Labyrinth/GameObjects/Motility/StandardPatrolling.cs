using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class StandardPatrolling : MonsterMotionBase
        {
        private Direction _currentDirection;

        public StandardPatrolling([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._currentDirection = initialDirection;
            }

        public override Direction GetDirection()
            {
            var result = GetDesiredDirection();
            this._currentDirection = result;
            return result;
            }

        private Direction GetDesiredDirection()
            {
            if (this.Monster.CanMoveInDirection(this._currentDirection))
                return this._currentDirection;
            var reversed = this._currentDirection.Reversed();
            if (this.Monster.CanMoveInDirection(reversed))
                return reversed;
            return Direction.None;
            }
        }
    }
