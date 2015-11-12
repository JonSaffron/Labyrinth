namespace Labyrinth.GameObjects.Movement
    {
    class CautiousPursuit : FullPursuit
        {
        public override Direction DetermineDirection(Monster monster)
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
            bool alterDirection = monster.Direction != Direction.None && (MonsterMovement.MonsterRandom.Next(256) & 3) == 0;
            Direction result = alterDirection 
                ? MonsterMovement.AlterDirection(monster.Direction) 
                : DetermineDirectionTowardsPlayer(monster).Reversed();
            return result;
            }

        private static Direction MoveTowardsPlayer(Monster monster)
            {
            bool alterDirection = (MonsterMovement.MonsterRandom.Next(256) & 7) == 0;
            Direction result = alterDirection 
                ? MonsterMovement.RandomDirection() 
                : DetermineDirectionTowardsPlayer(monster);
            return result;
            }
        }
    }
