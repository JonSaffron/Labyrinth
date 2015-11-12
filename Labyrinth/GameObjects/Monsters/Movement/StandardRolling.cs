namespace Labyrinth.GameObjects.Movement
    {
    class StandardRolling : StandardPatrolling
        {
        public StandardRolling(Direction initialDirection) : base(initialDirection)
            {
            }

        public StandardRolling()
            {
            }

        public override Direction DetermineDirection(Monster monster)
            {
            if (this.CurrentDirection == Direction.None)
                {
                this.CurrentDirection = MonsterMovement.RandomDirection();
                return this.CurrentDirection;
                }

            bool changeDirection = (MonsterMovement.MonsterRandom.Next(256) & 7) == 0;
            var result = changeDirection ? MonsterMovement.AlterDirection(this.CurrentDirection) : base.DetermineDirection(monster);
            this.CurrentDirection = result;

            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);

            return result;
            }
        }
    }
