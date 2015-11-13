using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    class StandardPatrolling : IMonsterMovement
        {
        private readonly Direction _initialPatrollingDirection;
        public Direction CurrentDirection { get; protected set; }

        public StandardPatrolling(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException("initialDirection", "May not be None");
            this._initialPatrollingDirection = initialDirection;
            }

        protected StandardPatrolling()
            {
            this.CurrentDirection = Direction.None;
            }

        public virtual Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = GetIntendedDirectionForPatrol(monster);
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        protected Direction GetIntendedDirectionForPatrol(Monster monster)
            {
            if (this.CurrentDirection == Direction.None)
                this.CurrentDirection = this._initialPatrollingDirection;

            if (this.CurrentDirection == Direction.None)
                throw new InvalidOperationException("Don't know which direction to patrol in!");

            bool canContinueMove = monster.CanMoveInDirection(this.CurrentDirection);
            if (canContinueMove)
                {
                TilePos tp = monster.TilePosition;
                TilePos pp = tp.GetPositionAfterOneMove(this.CurrentDirection);
                Vector2 potentiallyMovingTowards = pp.ToPosition();
                bool isInSameRoom = MonsterMovement.IsInSameRoom(monster.Position, potentiallyMovingTowards);
                if (isInSameRoom)
                    return this.CurrentDirection;
                }

            var result = this.CurrentDirection.Reversed();
            return result;
            }
        }
    }
