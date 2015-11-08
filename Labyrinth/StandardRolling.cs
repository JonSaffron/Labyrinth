using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class StandardRolling : StandardPatrolling, IMonsterMovement
        {
        public StandardRolling(Direction initialDirection) : base(initialDirection)
            {
            }

        public override Direction DetermineDirection(Monster monster)
            {
            if (monster.Direction == Direction.None)
                return MonsterMovement.RandomDirection();
            
            bool changeDirection = (MonsterMovement.MonsterRandom.Next(256) & 7) == 0;
            var result = changeDirection ? MonsterMovement.AlterDirection(monster.Direction) : base.DetermineDirection(monster);
            return result;
            }
        }
    }
