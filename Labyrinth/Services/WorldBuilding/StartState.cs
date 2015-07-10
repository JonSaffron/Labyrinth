namespace Labyrinth.Services.WorldBuilding
    {
    public class StartState
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
