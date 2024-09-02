using System.Collections.Generic;
using Labyrinth.DataStructures;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        string WorldName { get; }
        TilePos WorldSize { get; }
        Tile[,] FloorTiles { get; }
        bool RestartInSameRoom { get; }
        bool UnlockLevels { get; }
        Dictionary<int, PlayerStartState> PlayerStartStates { get; }
        List<RandomFruitDistribution> FruitDistributions { get; }

        void AddGameObjects(GameState gameState);
        }
    }
