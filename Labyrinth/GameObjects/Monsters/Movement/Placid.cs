using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    class Placid : MonsterMotionBase
        {
        public Placid([NotNull] Monster monster) : base(monster)
            {
            }

        public override Direction DetermineDirection()
            {
            // todo follow player if leaves room



            var result = MonsterMovement.RandomDirection();
            return result;
            }
        }
    }
