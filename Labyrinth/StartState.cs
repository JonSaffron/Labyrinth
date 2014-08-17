namespace Labyrinth
    {
    class StartState
        {
        public TilePos PlayerPosition { get; private set; }
        public int PlayerEnergy { get; private set; }
        public TilePos BlockPosition { get; private set; }
        
        public StartState(TilePos playerPosition, int playerEnergy, TilePos blockPosition)
            {
            this.PlayerPosition = playerPosition;
            this.PlayerEnergy = playerEnergy;
            this.BlockPosition = blockPosition;
            }
        }
    }
