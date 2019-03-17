using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IMovingItem : IGameObject
        {
        Movement CurrentMovement { get; }
        Vector2 OriginalPosition { get; set; }
        IBoundMovement MovementBoundary { get; } 

        bool Update(GameTime gameTime);

        void Move(Direction direction, decimal speed);
        void BounceBack(Direction direction, decimal speed);

        // todo are these required?
        // void ResetPosition(Vector2 position);
        // void StandStill();
        }
    }
