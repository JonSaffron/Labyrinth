namespace Labyrinth.GameObjects.Movement
    {
    class SemiAggressive : FullPursuit
        {
        public override Direction DetermineDirection(Monster monster)
            {
            bool makeAggressiveMove = (MonsterMovement.MonsterRandom.Next(256) & 1) == 0;
            if (monster.ChangeRooms != ChangeRooms.FollowsPlayer)
                makeAggressiveMove &= MonsterMovement.IsPlayerInSameRoomAsMonster(monster);
            var shouldFollowPlayer = makeAggressiveMove && MonsterMovement.IsPlayerInSight(monster);
            var result = shouldFollowPlayer ? DetermineDirectionTowardsPlayer(monster) : MonsterMovement.RandomDirection();
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }
        }
    }