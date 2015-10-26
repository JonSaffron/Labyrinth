using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);

        int GetMaximumWorldAreaId();
        StartState GetStartStateForWorldAreaId(int id);
        int GetWorldAreaIdForTilePos(TilePos tp);

        int Height { get; }
        int Width { get; }
        Tile this[TilePos tp] { get; }
        void GetGameObjects(GameState gameState);
        }
    }
