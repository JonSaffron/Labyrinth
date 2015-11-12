using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    class StandardPatrolling : IMonsterMovement
        {
        public Direction CurrentDirection { get; protected set; }

        public StandardPatrolling(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException("initialDirection", "May not be None");
            this.CurrentDirection = initialDirection;
            }

        protected StandardPatrolling()
            {
            this.CurrentDirection = Direction.None;
            }

        public virtual Direction DetermineDirection(Monster monster)
            {
            if (this.CurrentDirection == Direction.None)
                throw new InvalidOperationException("Don't know which direction to patrol in!");

            TilePos tp = monster.TilePosition;
            TilePos pp = tp.GetPositionAfterOneMove(this.CurrentDirection);
            bool isCurrentlyMovingTowardsFreeSpace = GlobalServices.GameState.CanTileBeOccupied(pp, true);
            Vector2 potentiallyMovingTowards = pp.ToPosition();
            bool isInSameRoom = MonsterMovement.IsInSameRoom(monster.Position, potentiallyMovingTowards);
            bool canContinueMovingInTheSameDirection = isCurrentlyMovingTowardsFreeSpace && isInSameRoom;
            var result = canContinueMovingInTheSameDirection ? this.CurrentDirection : this.CurrentDirection.Reversed();
            this.CurrentDirection = result;

            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);

            return result;
            }
        }
    }
