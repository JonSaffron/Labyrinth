namespace Labyrinth.Services.WorldBuilding
    {
    class StartState
        {
        public TilePos PlayerPosition { get; private set; }
        public int PlayerEnergy { get; private set; }
        
        public StartState(TilePos playerPosition, int playerEnergy)
            {
            this.PlayerPosition = playerPosition;
            this.PlayerEnergy = playerEnergy;
            }
        }
    }
