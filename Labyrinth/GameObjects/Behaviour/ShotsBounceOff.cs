using Labyrinth.GameObjects.Actions;

namespace Labyrinth.GameObjects.Actions
    {
    // todo This is a tricky one. It's not a behaviour of the monster per-se,
    // but I'm unclear what the best way to define it is

    class ShotsBounceOff : BaseBehaviour
        {
        public ShotsBounceOff()
            {
            this.Monster.ShotsBounceOff = true;
            }

        public ShotsBounceOff(Monster monster) : base(monster)
            {
            monster.ShotsBounceOff = true;
            }

        public override void Perform()
            {
            // nothing to do
            }
        }
    }
