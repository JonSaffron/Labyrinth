using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IGameObject
        {
        Vector2 Position { get; }
        bool IsExtant { get; }
        }
    }
