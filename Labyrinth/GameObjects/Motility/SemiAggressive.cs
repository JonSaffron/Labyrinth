using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class SemiAggressive : MonsterMotionBase
        {
        public SemiAggressive(Monster monster) : base(monster)
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
