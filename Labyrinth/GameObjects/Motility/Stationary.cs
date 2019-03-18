using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class Stationary : MonsterMotionBase
        {
        public Stationary([NotNull] Monster monster) : base(monster)
            {
            }

        public override bool SetDirectionAndDestination()
            {
            this.Monster.StandStill();
            return false;
            }
        }
    }
