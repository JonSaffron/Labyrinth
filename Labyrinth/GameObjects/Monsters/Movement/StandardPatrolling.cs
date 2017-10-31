using System;

namespace Labyrinth.GameObjects.Movement
    {
    class StandardPatrolling : IMonsterMovement
        {
        private readonly Direction _initialPatrollingDirection;
        private Direction _currentDirection;

        public StandardPatrolling(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._initialPatrollingDirection = initialDirection;
            }

        public Direction DetermineDirection(Monster monster)
            {
            if (this._currentDirection == Direction.None)
                this._currentDirection = this._initialPatrollingDirection;

            var intendedDirection = MonsterMovement.ContinueOrReverseWithinRoom(monster, this._currentDirection);
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this._currentDirection = result;
            return result;
            }
        }
    }
