namespace Labyrinth.GameObjects.Movement
    {
    class StandardRolling : IMonsterMovement
        {
        public Direction CurrentDirection { get; protected set; }

        public StandardRolling()
            {
            }

        public StandardRolling(Direction initialDirection)
            {
            this.CurrentDirection = initialDirection;
            }

        public virtual Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = GetIntendedDirection(monster);

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        protected Direction GetIntendedDirection(Monster monster)
            {
            if (this.CurrentDirection == Direction.None)
                this.CurrentDirection = MonsterMovement.RandomDirection();

            bool changeDirection = GlobalServices.Randomess.Test(7);
            var result = changeDirection ? MonsterMovement.AlterDirection(this.CurrentDirection) : MonsterMovement.ContinueOrReverseWithinRoom(monster, this.CurrentDirection);
            return result;
            }
        }
    }
