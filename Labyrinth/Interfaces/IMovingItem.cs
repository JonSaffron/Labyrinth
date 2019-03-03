using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IMovingItem : IGameObject
        {
        Movement CurrentMovement { get; }
        Vector2 OriginalPosition { get; set; }
        //bool IsMoving { get; }
        //ObjectCapability Capability { get; }
        decimal StandardSpeed { get; }
        IBoundMovement MovementBoundary { get; } 

        bool Update(GameTime gameTime);

        // void ResetPosition(Vector2 position);
        void Move(Direction direction, decimal speed);
        void BounceBack(Direction direction, decimal speed);
        //void StandStill();
        }
    }
