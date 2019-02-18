namespace Labyrinth.GameObjects.Actions
    {
    class Flitter : BaseBehaviour, IMovementBehaviour
        {
        private bool _doubleSpeed;

        public Flitter()
            {
            // nothing to do
            }

        public Flitter(Monster monster): base(monster)
            {
            // nothing to do
            }

        public override void Perform()
            {
            this._doubleSpeed = !this._doubleSpeed;
            this.Monster.CurrentSpeed = Constants.BaseSpeed << (this._doubleSpeed ? 1 : 0);
            }
        }
    }
