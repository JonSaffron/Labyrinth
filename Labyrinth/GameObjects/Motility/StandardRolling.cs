using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class StandardRolling : MonsterMotionBase
        {
        protected Direction CurrentDirection { get; private set; }

        [UsedImplicitly]
        public StandardRolling([NotNull] Monster monster) : base(monster)
            {
            }

        public StandardRolling([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            this.CurrentDirection = initialDirection;
            }

        protected override Direction DetermineDirection()
            {
            if (this.CurrentDirection == Direction.None)
                {
                return ShouldMonsterFollowPlayerIntoAnotherRoom() 
                    ? this.Monster.DetermineDirectionTowardsPlayer()
                    : MonsterMovement.RandomDirection();
                }

            bool shouldChangeDirection = GlobalServices.Randomness.Test(7);
            if (shouldChangeDirection)
                {
                return ShouldMonsterFollowPlayerIntoAnotherRoom() 
                    ? this.Monster.DetermineDirectionTowardsPlayer()
                    : MonsterMovement.AlterDirectionByVeeringAway(this.CurrentDirection);
                }

            if (this.Monster.CanMoveInDirection(this.CurrentDirection))
                return this.CurrentDirection;

            var reversed = this.CurrentDirection.Reversed();
            return reversed;
            }

        private bool ShouldMonsterFollowPlayerIntoAnotherRoom()
            {
            bool shouldMonsterFollowPlayerIntoAnotherRoom =
                this.Monster.ChangeRooms == ChangeRooms.FollowsPlayer
                && !this.Monster.IsPlayerInSameRoom()
                && this.Monster.IsPlayerNearby();
            return shouldMonsterFollowPlayerIntoAnotherRoom;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            this.Monster.ConfirmDirectionToMoveIn(direction, out Direction feasibleDirection);
            direction = feasibleDirection;

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction);
            this.CurrentDirection = direction;
            return true;
            }
        }
    }
