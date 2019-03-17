using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    class StandardRolling : MonsterMotionBase
        {
        protected Direction CurrentDirection { get; private set; }

        public StandardRolling([NotNull] Monster monster) : base(monster)
            {
            }

        public StandardRolling([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            this.CurrentDirection = initialDirection;
            }

        public override Direction DetermineDirection()
            {
            if (this.Monster.ChangeRooms == ChangeRooms.FollowsPlayer 
                && !MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster) 
                && MonsterMovement.IsPlayerNearby(this.Monster))
                {
                var pursuitResult = CautiousPursuit.MoveTowardsPlayer(this.Monster);
                return pursuitResult;
                }

            if (this.CurrentDirection == Direction.None)
                this.CurrentDirection = MonsterMovement.RandomDirection();

            bool changeDirection = GlobalServices.Randomness.Test(7);
            if (changeDirection)
                {
                var result = MonsterMovement.AlterDirection(this.CurrentDirection);
                return result;
                }

            if (this.Monster.CanMoveInDirection(this.CurrentDirection))
                return this.CurrentDirection;

            var reversed = this.CurrentDirection.Reversed();
            return reversed;
            }

        public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            direction = MonsterMovement.UpdateDirectionWhereMovementBlocked(this.Monster, direction);

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            this.CurrentDirection = direction;
            return true;
            }
        }
    }
