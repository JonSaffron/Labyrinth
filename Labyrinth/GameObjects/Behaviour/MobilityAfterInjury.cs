namespace Labyrinth.GameObjects.Behaviour
    {
    class MobilityAfterInjury : BaseBehaviour, IInjuryBehaviour
        {
        private readonly MonsterMobility _mobilityToChangeTo;

        public MobilityAfterInjury(Monster monster, MonsterMobility mobilityToChangeTo) : base(monster)
            {
            this._mobilityToChangeTo = mobilityToChangeTo;
            }

        public override void Perform()
            {
            this.Monster.Mobility = this._mobilityToChangeTo;
            this.Monster.Behaviours.Remove<MobilityAfterInjury>();
            }
        }
    }
