﻿namespace Labyrinth.GameObjects.Movement
    {
    class CautiousPursuit : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            bool isScaredOfPlayer = IsScaredOfPlayer(monster);
            var result = isScaredOfPlayer ? MoveAwayFromPlayer(monster) : MoveTowardsPlayer(monster);
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }

        private static bool IsScaredOfPlayer(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        private static Direction MoveAwayFromPlayer(Monster monster)
            {
            var towardsPlayer = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            bool alterDirection = GlobalServices.Randomess.Test(3);
            Direction result = alterDirection 
                ?  MonsterMovement.AlterDirection(towardsPlayer) 
                : towardsPlayer.Reversed();
            return result;
            }

        private static Direction MoveTowardsPlayer(Monster monster)
            {
            bool alterDirection = GlobalServices.Randomess.Test(7);
            Direction result = alterDirection 
                ? MonsterMovement.RandomDirection() 
                : MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            return result;
            }
        }
    }
