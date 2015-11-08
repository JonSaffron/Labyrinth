using System;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class StandardPatrolling : IMonsterMovement
        {
        public Direction CurrentDirection { get; private set; }

        public StandardPatrolling(Direction initialDirection)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException("initialDirection", "May not be None");
            this.CurrentDirection = initialDirection;
            }

        public virtual Direction DetermineDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            TilePos pp = tp.GetPositionAfterOneMove(this.CurrentDirection);
            bool isCurrentlyMovingTowardsFreeSpace = GlobalServices.GameState.CanTileBeOccupied(pp, true);
            Vector2 potentiallyMovingTowards = pp.ToPosition();
            bool isInSameRoom = MonsterMovement.IsInSameRoom(monster.Position, potentiallyMovingTowards);
            bool canContinueMovingInTheSameDirection = isCurrentlyMovingTowardsFreeSpace && isInSameRoom;
            var result = canContinueMovingInTheSameDirection ? monster.Direction : monster.Direction.Reversed();
            System.Diagnostics.Trace.WriteLine(string.Format("{0} {1} {2} {3} {4}", tp, pp, isCurrentlyMovingTowardsFreeSpace, isInSameRoom, result));
            this.CurrentDirection = result;
            return result;
            }
        }
    }
