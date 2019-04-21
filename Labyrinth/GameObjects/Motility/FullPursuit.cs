using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class FullPursuit : MonsterMotionBase
        {
        // Determine which direction gets closer to player
        // Horizontal movement is preferred, but if the move isn't possible or the monster is already
        // on the same column as the player then vertical movement will be tried.
        // 
        public FullPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        protected override Direction DetermineDirection()
            {
            Direction directionTowardsPlayer = this.Monster.DetermineDirectionTowardsPlayer();
            bool canMoveTowardsPlayer = this.Monster.ConfirmDirectionToMoveIn(directionTowardsPlayer, out Direction feasibleDirection);
            if (!canMoveTowardsPlayer || feasibleDirection == Direction.None)
                return feasibleDirection;

            TilePos potentiallyMovingTowards = this.Monster.TilePosition.GetPositionAfterOneMove(directionTowardsPlayer);
            if (potentiallyMovingTowards != GlobalServices.GameState.Player.TilePosition)
                return directionTowardsPlayer;

            var directionAwayFromPlayer = MonsterMovement.AlterDirectionByVeeringAway(feasibleDirection);
            this.Monster.ConfirmDirectionToMoveIn(directionAwayFromPlayer, out Direction result);
            return result;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction);
            return true;
            }
        }
    }
