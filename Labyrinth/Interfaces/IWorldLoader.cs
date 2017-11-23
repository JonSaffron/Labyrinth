using System.Collections.Generic;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);

        TilePos WorldSize { get; }
        Tile[,] FloorTiles { get; }
        bool RestartInSameRoom { get; }
        bool ReplenishFruit { get; }
        Dictionary<int, PlayerStartState> PlayerStartStates { get; }
        List<RandomFruitDistribution> FruitDistributions { get; }

        void AddGameObjects(GameState gameState);
        }
    }
