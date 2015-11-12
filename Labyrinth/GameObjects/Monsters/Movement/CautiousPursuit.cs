using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class CautiousPursuit : FullPursuit
        {
        public override Direction DetermineDirection(Monster monster)
            {
            bool isCautious = IsCautious(monster);
            var result = isCautious ? MoveAwayFromPlayer(monster) : MoveTowardsPlayer(monster);
            return result;
            }

        private static bool IsCautious(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        private static Direction MoveAwayFromPlayer(Monster monster)
            {
            Direction result;
            bool alterDirection = monster.Direction != Direction.None && (MonsterMovement.MonsterRandom.Next(256) & 3) == 0;
            if (alterDirection)
                result = MonsterMovement.AlterDirection(monster.Direction);
            else
                {
                var d = DetermineDirectionTowardsPlayer(monster);
                result = (d == Direction.None) ? MonsterMovement.RandomDirection() : d.Reversed();
                }
            return result;
            }

        private static Direction MoveTowardsPlayer(Monster monster)
            {
            Direction result;
            if ((MonsterMovement.MonsterRandom.Next(256) & 7) == 0)
                {
                result = MonsterMovement.RandomDirection();
                }
            else
                {
                result = DetermineDirectionTowardsPlayer(monster);
                if (result == Direction.None)
                    result = MonsterMovement.RandomDirection();
                }
            return result;
            }
        }
    }