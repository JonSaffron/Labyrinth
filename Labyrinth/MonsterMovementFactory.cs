using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class MonsterMovementFactory
        {
        IMonsterMovement StandardPatrolling(Direction initialDirection)
            {
            var result = new StandardPatrolling(initialDirection);
            return result;
            }

        IMonsterMovement StandardRolling(Direction inititalDirection)
            {
            var result = new StandardRolling(inititalDirection);
            return result;
            }

        IMonsterMovement Cautious()
            {
            var result = new Cautious();
            return result;
            }
        }

    class Cautious : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            bool isCautious = IsCautious(monster);
            var result = isCautious ? DetermineDirectionAwayFromPlayer(monster) : DetermineDirectionTowardsPlayer(monster);
            return result;
            }

        private static bool IsCautious(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        private static Direction DetermineDirectionAwayFromPlayer(Monster monster)
            {
            Direction result;
            bool alterDirection = monster.Direction != Direction.None && (MonsterMovement.MonsterRandom.Next(256) & 3) == 0;
            if (alterDirection)
                result = MonsterMovement.AlterDirection(monster.Direction);
            else
                {
                var d = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
                result = (d == Direction.None) ? MonsterMovement.RandomDirection() : d.Reversed();
                }
            return result;
            }

        private static Direction DetermineDirectionTowardsPlayer(Monster monster)
            {
            Direction result;
            if ((MonsterMovement.MonsterRandom.Next(256) & 7) == 0)
                {
                result = MonsterMovement.RandomDirection();
                }
            else
                {
                result = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
                if (result == Direction.None)
                    result = MonsterMovement.RandomDirection();
                }
            return result;
            }
        }
    }
