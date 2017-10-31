namespace Labyrinth.GameObjects.Movement
    {
    class FullPursuit : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            Direction intendedDirection = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            var alternativeDirectionWhenBlocked = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            if (alternativeDirectionWhenBlocked == Direction.None || alternativeDirectionWhenBlocked != intendedDirection)
                return alternativeDirectionWhenBlocked;

            TilePos potentiallyMovingTowards = monster.TilePosition.GetPositionAfterOneMove(intendedDirection);
            if (potentiallyMovingTowards == GlobalServices.GameState.Player.TilePosition)
                intendedDirection = MonsterMovement.AlterDirection(intendedDirection);

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            return result;
            }
        }
    }
