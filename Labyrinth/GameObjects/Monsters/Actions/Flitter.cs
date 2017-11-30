namespace Labyrinth.GameObjects.Actions
    {
    class Flitter : BaseBehaviour
        {
        private bool _doubleSpeed;

        public override void Perform()
            {
            this._doubleSpeed = !this._doubleSpeed;
            this.Monster.CurrentSpeed = Constants.BaseSpeed << (this._doubleSpeed ? 1 : 0);
            }
        }
    }
