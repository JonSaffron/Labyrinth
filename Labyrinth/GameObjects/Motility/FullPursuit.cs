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

        public override Direction GetDirection()
            {
            var directionTowardsPlayer = this.Monster.DetermineDirectionTowardsPlayer();
            if (!directionTowardsPlayer.IsConfirmedSafe)
                {
                bool canReallyMoveTowardsPlayer = this.Monster.ConfirmDirectionToMoveIn(directionTowardsPlayer.Direction, out Direction feasibleDirection);
                if (!canReallyMoveTowardsPlayer || feasibleDirection == Direction.None)
                    return feasibleDirection;
                }

            // not allowed to actually step onto the same tile as the player
            TilePos potentiallyMovingTowards = this.Monster.TilePosition.GetPositionAfterOneMove(directionTowardsPlayer.Direction);
            if (potentiallyMovingTowards != GlobalServices.GameState.Player.TilePosition)
                return directionTowardsPlayer.Direction;

            var directionAwayFromPlayer = MonsterMovement.AlterDirectionByVeeringAway(directionTowardsPlayer.Direction);
            this.Monster.ConfirmDirectionToMoveIn(directionAwayFromPlayer.Direction, out Direction result);
            return result;
            }
        }
    }
