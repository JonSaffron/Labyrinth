namespace Labyrinth.GameObjects.Movement
    {
    class SemiAggressive : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            bool makeAggressiveMove = GlobalServices.Randomess.Test(1);
            if (monster.ChangeRooms != ChangeRooms.FollowsPlayer)
                makeAggressiveMove &= MonsterMovement.IsPlayerInSameRoomAsMonster(monster);
            var shouldFollowPlayer = makeAggressiveMove && MonsterMovement.IsPlayerInSight(monster);
            var result = shouldFollowPlayer ? MonsterMovement.DetermineDirectionTowardsPlayer(monster) : MonsterMovement.RandomDirection();
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }
        }
    }
