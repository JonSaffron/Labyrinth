﻿using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IMovingItem : IGameObject
        {
        Movement CurrentMovement { get; }
        Vector2 OriginalPosition { get; set; }
        IBoundMovement MovementBoundary { get; } 

        void Move(Direction direction, MovementType movementType);

        // todo are these required?
        // void ResetPosition(Vector2 position);
        // void StandStill();
        }
    }
