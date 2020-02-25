using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    class Stationary : MonsterMotionBase
        {
        public Stationary([NotNull] Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            return ConfirmedDirection.None;
            }
        }
    }
