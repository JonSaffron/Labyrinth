using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class Placid : MonsterMotionBase
        {
        public Placid([NotNull] Monster monster) : base(monster)
            {
            }

        protected override Direction DetermineDirection()
            {
            if (this.Monster.ChangeRooms == ChangeRooms.FollowsPlayer 
                && !MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster) 
                && MonsterMovement.IsPlayerNearby(this.Monster))
                {
                var pursuitResult = CautiousPursuit.MoveTowardsPlayer(this.Monster);
                return pursuitResult;
                }

            var result = MonsterMovement.RandomDirection();
            return result;
            }
        }
    }
