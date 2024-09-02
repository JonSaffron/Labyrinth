using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class Placid : MonsterMotionBase
        {
        public Placid(Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            IDirectionChosen direction = GetDesiredDirection();
            return base.GetConfirmedDirection(direction);
            }

        private IDirectionChosen GetDesiredDirection()
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
