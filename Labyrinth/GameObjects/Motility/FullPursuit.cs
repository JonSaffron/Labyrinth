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
        public FullPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            IDirectionChosen directionTowardsPlayer = this.Monster.DetermineDirectionTowardsPlayer();
            if (!(directionTowardsPlayer is ConfirmedDirection))
                {
                bool notPossibleToGoInDirectionTowardsPlayer = !this.Monster.ConfirmDirectionToMoveIn(directionTowardsPlayer, out ConfirmedDirection feasibleDirection);
                if (notPossibleToGoInDirectionTowardsPlayer || feasibleDirection == Direction.None)
                    return feasibleDirection;
                }

            // we know at this point that we could move in DirectionTowardsPlayer HOWEVER we don't want to actually step onto the same tile as the player
            TilePos potentiallyMovingTowards = this.Monster.TilePosition.GetPositionAfterOneMove(directionTowardsPlayer.Direction);
            if (potentiallyMovingTowards != GlobalServices.GameState.Player.TilePosition)
                {
                // it's okay. we'll be closer to the player without actually touching the player
                return directionTowardsPlayer.Confirm();
                }

            var directionAwayFromPlayer = MonsterMovement.AlterDirectionByVeeringAway(directionTowardsPlayer.Direction);
            this.Monster.ConfirmDirectionToMoveIn(directionAwayFromPlayer, out ConfirmedDirection result);
            return result;
            }
        }
    }
