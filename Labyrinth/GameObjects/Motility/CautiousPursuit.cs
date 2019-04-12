using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class CautiousPursuit : MonsterMotionBase
        {
        public CautiousPursuit([NotNull] Monster monster) : base(monster)
            {
            }

        protected override Direction DetermineDirection()
            {
            var method = IsScaredOfPlayer(this.Monster) 
                ? (Func<Monster, Direction>) MoveAwayFromPlayer 
                                           : MoveTowardsPlayer;
            return method(this.Monster);
            }

        private static bool IsScaredOfPlayer(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        public static Direction MoveTowardsPlayer(Monster monster)
            {
            bool shouldMoveRandomly = GlobalServices.Randomness.Test(7);
            Direction result = shouldMoveRandomly
                ? MonsterMovement.RandomDirection() 
                : monster.DetermineDirectionTowardsPlayer();
            return result;
            }

        private static Direction MoveAwayFromPlayer(Monster monster)
            {
            var awayFromPlayer = monster.DetermineDirectionAwayFromPlayer();
            bool shouldDodgeWhilstFleeing = GlobalServices.Randomness.Test(3);
            if (shouldDodgeWhilstFleeing)
                {
                var alteredDirection = MonsterMovement.AlterDirectionByVeeringAway(awayFromPlayer);
                return alteredDirection;
                }
            
            return awayFromPlayer;
            }
        }
    }
