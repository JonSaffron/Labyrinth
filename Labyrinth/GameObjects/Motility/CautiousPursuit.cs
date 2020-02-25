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

        public override ConfirmedDirection GetDirection()
            {
            var method = IsScaredOfPlayer(this.Monster) 
                ? (Func<Monster, IDirectionChosen>) MoveAwayFromPlayer : MoveTowardsPlayer;
            var selectedDirection = method(this.Monster);
            return GetConfirmedDirection(selectedDirection);
            }

        private static bool IsScaredOfPlayer(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
            return result;
            }

        public static IDirectionChosen MoveTowardsPlayer(Monster monster)
            {
            bool shouldMoveRandomly = GlobalServices.Randomness.Test(7);
            IDirectionChosen result = shouldMoveRandomly
                ? MonsterMovement.RandomDirection() 
                : monster.DetermineDirectionTowardsPlayer();
            return result;
            }

        private static IDirectionChosen MoveAwayFromPlayer(Monster monster)
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
