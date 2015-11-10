using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class StandardRolling : StandardPatrolling
        {
        public StandardRolling(Direction initialDirection) : base(initialDirection)
            {
            }

        protected StandardRolling()
            {
            }

        public override Direction DetermineDirection(Monster monster)
            {
            if (monster.Direction == Direction.None)
                {
                this.CurrentDirection = MonsterMovement.RandomDirection();
                return this.CurrentDirection;
                }

            bool changeDirection = (MonsterMovement.MonsterRandom.Next(256) & 7) == 0;
            var result = changeDirection ? MonsterMovement.AlterDirection(monster.Direction) : base.DetermineDirection(monster);
            return result;
            }
        }
    }
