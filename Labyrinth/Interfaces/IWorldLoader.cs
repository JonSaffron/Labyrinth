using System.Collections.Generic;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);

        TilePos WorldSize { get; }
        Tile[,] GetFloorTiles();
        bool RestartInSameRoom { get; }
        Dictionary<int, PlayerStartState> GetPlayerStartStates(); 

        void AddGameObjects(GameState gameState);
        }
    }
