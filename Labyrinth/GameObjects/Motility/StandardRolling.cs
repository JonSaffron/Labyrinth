using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class StandardRolling : MonsterMotionBase
        {
        protected Direction CurrentDirection { get; private set; }

        public StandardRolling(Monster monster) : base(monster)
            {
            }

        public StandardRolling(Monster monster, Direction initialDirection) : base(monster)
            {
            this.CurrentDirection = initialDirection;
            }

        public override ConfirmedDirection GetDirection()
            {
            IDirectionChosen desiredDirection = GetDesiredDirection();
            var result = GetConfirmedDirection(desiredDirection);
            this.CurrentDirection = result;
            return result;
            }

        protected IDirectionChosen GetDesiredDirection()
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
                return new ConfirmedDirection(this.CurrentDirection);

            var reversed = this.CurrentDirection.Reversed();
            return new PossibleDirection(reversed);
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
