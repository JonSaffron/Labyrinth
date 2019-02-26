namespace Labyrinth.GameObjects.Behaviour
    {
    class ChangeMovementWhenHurt : BaseBehaviour, IInjuryBehaviour
        {
        private readonly MonsterMobility _mobilityToChangeTo;

        public ChangeMovementWhenHurt(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public ChangeMovementWhenHurt()
            {
            // nothing to do
            }

        public ChangeMovementWhenHurt(MonsterMobility mobilityToChangeTo)
            {
            this._mobilityToChangeTo = mobilityToChangeTo;
            }

        public override void Perform()
            {
            this.Monster.Mobility = this._mobilityToChangeTo;
            this.Monster.Behaviours.Remove<ChangeMovementWhenHurt>();
            }
        }
    }
