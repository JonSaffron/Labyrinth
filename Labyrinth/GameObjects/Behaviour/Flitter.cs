namespace Labyrinth.GameObjects.Behaviour
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
            this.Monster.SpeedAdjustmentFactor = (this._doubleSpeed ? 2 : 1);
            }
        }
    }
