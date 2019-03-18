using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class CautiousPursuit : MonsterMotionBase
        {
        public CautiousPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        private Direction _currentDirection;

        public override Direction DetermineDirection()
            {
            var result = IsScaredOfPlayer(this.Monster) 
                    ? MoveAwayFromPlayer(this.Monster) 
                    : MoveTowardsPlayer(this.Monster);
            return result;
            }

        private static bool IsScaredOfPlayer(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        private Direction MoveAwayFromPlayer(Monster monster)
            {
            var towardsPlayer = MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            bool alterDirection = GlobalServices.Randomness.Test(3);
            Direction result = !alterDirection || this._currentDirection == Direction.None
                ? towardsPlayer.Reversed() 
                : MonsterMovement.AlterDirection(this._currentDirection);
            return result;
            }

        public static Direction MoveTowardsPlayer(Monster monster)
            {
            bool alterDirection = GlobalServices.Randomness.Test(7);
            Direction result = alterDirection 
                ? MonsterMovement.RandomDirection() 
                : MonsterMovement.DetermineDirectionTowardsPlayer(monster);
            return result;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            if (direction != Direction.None)
                {
                direction = MonsterMovement.UpdateDirectionWhereMovementBlocked(this.Monster, direction);
                }

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            this._currentDirection = direction;
            return true;
            }
        }
    }
