using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    class SemiAggressive : MonsterMotionBase
        {
        public SemiAggressive([NotNull] Monster monster) : base(monster)
            {
            }

        public override Direction DetermineDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? MonsterMovement.DetermineDirectionTowardsPlayer(this.Monster) 
                    : MonsterMovement.RandomDirection();
            
            return result;
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            bool result = GlobalServices.Randomess.Test(1) && MonsterMovement.IsPlayerNearby(monster);
            return result;
            }   
        }
    }
