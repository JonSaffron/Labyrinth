using System.Collections.Generic;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);
        bool RestartInSameRoom { get; }
        Tile[,] GetFloorTiles();
        void AddGameObjects(GameState gameState);
        Dictionary<int, PlayerStartState> GetPlayerStartStates(); 
        }
    }
