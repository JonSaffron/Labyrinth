using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    class FullPursuit : MonsterMotionBase
        {
        public FullPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        public override Direction DetermineDirection()
            {
            Direction intendedDirection = MonsterMovement.DetermineDirectionTowardsPlayer(this.Monster);
            var alternativeDirectionWhenBlocked = MonsterMovement.UpdateDirectionWhereMovementBlocked(this.Monster, intendedDirection);
            if (alternativeDirectionWhenBlocked == Direction.None || alternativeDirectionWhenBlocked != intendedDirection)
                return alternativeDirectionWhenBlocked;

            TilePos potentiallyMovingTowards = this.Monster.TilePosition.GetPositionAfterOneMove(intendedDirection);
            if (potentiallyMovingTowards == GlobalServices.GameState.Player.TilePosition)
                intendedDirection = MonsterMovement.AlterDirection(intendedDirection);

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(this.Monster, intendedDirection);
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

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            return true;
            }
        }
    }
