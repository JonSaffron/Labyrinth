using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class StandardPatrolling : MonsterMotionBase
        {
        private Direction _currentDirection;

        public StandardPatrolling(Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._currentDirection = initialDirection;
            }

        public override ConfirmedDirection GetDirection()
            {
            var result = GetConfirmedDirection();
            this._currentDirection = result;
            return result;
            }

        private ConfirmedDirection GetConfirmedDirection()
            {
            if (this.Monster.CanMoveInDirection(this._currentDirection))
                return new ConfirmedDirection(this._currentDirection);
            var reversed = this._currentDirection.Reversed();
            if (this.Monster.CanMoveInDirection(reversed))
                return new ConfirmedDirection(reversed);
            return ConfirmedDirection.None;
            }
        }
    }
