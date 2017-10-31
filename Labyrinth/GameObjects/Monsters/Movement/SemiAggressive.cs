namespace Labyrinth.GameObjects.Movement
    {
    class SemiAggressive : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = 
                ShouldMakeAnAggressiveMove(monster) 
                    ? MonsterMovement.DetermineDirectionTowardsPlayer(monster) 
                    : MonsterMovement.RandomDirection();
            
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            return result;
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            bool result = GlobalServices.Randomess.Test(1) && MonsterMovement.IsPlayerNearby(monster);
            if (monster.ChangeRooms != ChangeRooms.FollowsPlayer && !MonsterMovement.IsPlayerInSameRoomAsMonster(monster))
                result = false;
            return result;
            }   
        }
    }
