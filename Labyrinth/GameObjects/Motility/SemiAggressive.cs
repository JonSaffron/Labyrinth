using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
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
            bool result = GlobalServices.Randomness.Test(1) && MonsterMovement.IsPlayerNearby(monster);
            return result;
            }   
        }
    }
