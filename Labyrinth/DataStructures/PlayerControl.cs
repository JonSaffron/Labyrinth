namespace Labyrinth.DataStructures  
    {
    public readonly struct PlayerControl
        {
        public readonly Direction Direction;
        public readonly FiringState FireState1;
        public readonly FiringState FireState2;

        public static readonly PlayerControl NoAction = new PlayerControl(Direction.None, FiringState.None, FiringState.None);

        public PlayerControl(Direction direction, FiringState fireState1, FiringState fireState2)
            {
            this.Direction = direction;
            this.FireState1 = fireState1;
            this.FireState2 = fireState2;
            }
        }   
    }
