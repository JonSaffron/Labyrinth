using System.Collections.Generic;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);
        Tile[,] GetFloorTiles();
        void GetGameObjects(GameState gameState);
        Dictionary<int, PlayerStartState> GetPlayerStartStates(); 
        }
    }
