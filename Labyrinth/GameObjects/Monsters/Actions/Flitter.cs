namespace Labyrinth.GameObjects.Monsters.Actions
    {
    class Flitter : BaseAction
        {
        private bool _doubleSpeed;

        public override void PerformAction()
            {
            this._doubleSpeed = !this._doubleSpeed;
            this.Monster.CurrentSpeed = Constants.BaseSpeed << (this._doubleSpeed ? 1 : 0);
            }
        }
    }
