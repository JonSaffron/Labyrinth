using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    internal class Stationary : MonsterMotionBase
        {
        public Stationary(Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            return ConfirmedDirection.None;
            }
        }
    }
