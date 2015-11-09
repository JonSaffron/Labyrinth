using Labyrinth.GameObjects;

namespace Labyrinth
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
            return result;
            }
        }
    }