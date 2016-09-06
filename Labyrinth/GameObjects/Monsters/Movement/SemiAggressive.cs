namespace Labyrinth.GameObjects.Movement
    {
    // todo: this should not inherit from FullPursuit

    class SemiAggressive : FullPursuit
        {
        public override Direction DetermineDirection(Monster monster)
            {
            bool makeAggressiveMove = GlobalServices.Randomess.Test(1);
            if (monster.ChangeRooms != ChangeRooms.FollowsPlayer)
                makeAggressiveMove &= MonsterMovement.IsPlayerInSameRoomAsMonster(monster);
            var shouldFollowPlayer = makeAggressiveMove && MonsterMovement.IsPlayerInSight(monster);
            var result = shouldFollowPlayer ? DetermineDirectionTowardsPlayer(monster) : MonsterMovement.RandomDirection();
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }
        }
    }
