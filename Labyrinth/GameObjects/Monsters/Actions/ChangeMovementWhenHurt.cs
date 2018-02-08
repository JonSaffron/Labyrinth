namespace Labyrinth.GameObjects.Actions
    {
    class ChangeMovementWhenHurt : BaseBehaviour
        {
        private readonly MonsterMobility _mobilityToChangeTo;

        public ChangeMovementWhenHurt(MonsterMobility mobilityToChangeTo)
            {
            _mobilityToChangeTo = mobilityToChangeTo;
            }

        public override void Perform()
            {
            this.Monster.Mobility = this._mobilityToChangeTo;
            this.Monster.Behaviours.Remove<ChangeMovementWhenHurt>();
            }
        }
    }
