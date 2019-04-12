using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class SemiAggressive : MonsterMotionBase
        {
        public SemiAggressive([NotNull] Monster monster) : base(monster)
            {
            }

        protected override Direction DetermineDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? this.Monster.DetermineDirectionTowardsPlayer() 
                    : MonsterMovement.RandomDirection();
            
            return result;
            }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            return GlobalServices.Randomness.Test(1) && MonsterMovement.IsPlayerNearby(monster);
            }   
        }
    }
