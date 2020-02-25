using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class SemiAggressive : MonsterMotionBase
        {
        public SemiAggressive([NotNull] Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? this.Monster.DetermineDirectionTowardsPlayer() 
                    : MonsterMovement.RandomDirection();
            
            return base.GetConfirmedDirection(result);
            }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            return GlobalServices.Randomness.Test(1) && monster.IsPlayerNearby();
            }   
        }
    }
