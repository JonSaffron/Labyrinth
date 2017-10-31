namespace Labyrinth.GameObjects.Movement
    {
    class FullPursuit : IMonsterMovement
        {
        public virtual Direction DetermineDirection(Monster monster)
            {
            Direction result = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            if (result == Direction.None)
                return result;

            TilePos potentiallyMovingTowards = monster.TilePosition.GetPositionAfterOneMove(result);
            if (potentiallyMovingTowards == GlobalServices.GameState.Player.TilePosition)
                result = MonsterMovement.AlterDirection(result);

            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }
        }
    }
