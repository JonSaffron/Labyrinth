using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IGameObject
        {
        Vector2 Position { get; }
        TilePos TilePosition { get; }
        bool IsExtant { get; }
        }
    }
