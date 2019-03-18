namespace Labyrinth.GameObjects.Behaviour
    {
    class ChangeRoomsAfterInjury : BaseBehaviour, IInjuryBehaviour
        {
        private readonly ChangeRooms _changeRooms;

        public ChangeRoomsAfterInjury(Monster monster, ChangeRooms changeRooms) : base(monster)
            {
            this._changeRooms = changeRooms;
            }

        public override void Perform()
            {
            this.Monster.ChangeRooms = this._changeRooms;
            this.Monster.Behaviours.Remove<ChangeRoomsAfterInjury>();
            }
        }
    }
