using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class Placid : MonsterMotionBase
        {
        public Placid([NotNull] Monster monster) : base(monster)
            {
            }

        public override Direction GetDirection()
            {
            SelectedDirection direction = GetDesiredDirection();
            return base.GetConfirmedSafeDirection(direction);
            }

        private SelectedDirection GetDesiredDirection()
            {
            if (this.Monster.ChangeRooms == ChangeRooms.FollowsPlayer 
                && !this.Monster.IsPlayerInSameRoom() 
                && this.Monster.IsPlayerNearby())
                {
                var pursuitResult = CautiousPursuit.MoveTowardsPlayer(this.Monster);
                return pursuitResult;
                }

            var result = MonsterMovement.RandomDirection();
            return result;
            }
        }
    }
