using JetBrains.Annotations;
using Labyrinth.DataStructures;

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

        public override Direction GetDirection()
            {
            SelectedDirection desiredDirection = GetDesiredDirection();
            var result = GetConfirmedSafeDirection(desiredDirection);
            this.CurrentDirection = result;
            return result;
            }

        protected SelectedDirection GetDesiredDirection()
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
                return SelectedDirection.SafeDirection(this.CurrentDirection);

            var reversed = this.CurrentDirection.Reversed();
            return SelectedDirection.UnsafeDirection(reversed);
            }

        private bool ShouldMonsterFollowPlayerIntoAnotherRoom()
            {
            bool shouldMonsterFollowPlayerIntoAnotherRoom =
                this.Monster.ChangeRooms == ChangeRooms.FollowsPlayer
                && !this.Monster.IsPlayerInSameRoom()
                && this.Monster.IsPlayerNearby();
            return shouldMonsterFollowPlayerIntoAnotherRoom;
            }
        }
    }
