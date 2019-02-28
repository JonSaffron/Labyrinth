namespace Labyrinth.GameObjects.Behaviour
    {
    class ChangeMovementWhenHurt : BaseBehaviour, IInjuryBehaviour
        {
        private readonly MonsterMobility _mobilityToChangeTo;
        private readonly ChangeRooms _changeRooms;

        public ChangeMovementWhenHurt(Monster monster, MonsterMobility mobilityToChangeTo, ChangeRooms changeRooms) : base(monster)
            {
            this._mobilityToChangeTo = mobilityToChangeTo;
            this._changeRooms = changeRooms;
            }

        public override void Perform()
            {
            this.Monster.Mobility = this._mobilityToChangeTo;
            this.Monster.ChangeRooms = this._changeRooms;
            this.Monster.Behaviours.Remove<ChangeMovementWhenHurt>();
            }
        }
    }
