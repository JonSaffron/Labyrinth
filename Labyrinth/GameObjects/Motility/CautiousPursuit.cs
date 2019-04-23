using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class CautiousPursuit : MonsterMotionBase
        {
        public CautiousPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        public override Direction GetDirection()
            {
            var method = IsScaredOfPlayer(this.Monster) 
                ? (Func<Monster, SelectedDirection>) MoveAwayFromPlayer 
                                           : MoveTowardsPlayer;
            var selectedDirection = method(this.Monster);
            return GetConfirmedSafeDirection(selectedDirection);
            }

        private static bool IsScaredOfPlayer(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        public static SelectedDirection MoveTowardsPlayer(Monster monster)
            {
            bool shouldMoveRandomly = GlobalServices.Randomness.Test(7);
            SelectedDirection result = shouldMoveRandomly
                ? MonsterMovement.RandomDirection() 
                : monster.DetermineDirectionTowardsPlayer();
            return result;
            }

        private static SelectedDirection MoveAwayFromPlayer(Monster monster)
            {
            var awayFromPlayer = monster.DetermineDirectionAwayFromPlayer();
            bool shouldDodgeWhilstFleeing = GlobalServices.Randomness.Test(3);
            if (shouldDodgeWhilstFleeing)
                {
                var alteredDirection = MonsterMovement.AlterDirectionByVeeringAway(awayFromPlayer.Direction);
                return alteredDirection;
                }
            
            return awayFromPlayer;
            }
        }
    }
