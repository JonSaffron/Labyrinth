using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public readonly struct Movement
        {
        public static Movement Still { get; } = new Movement();
        public Direction Direction { get; }
        public decimal Velocity { get; }

        private readonly Vector2? _movingTowards;

        public Movement(Direction direction, Vector2 movingTowards, decimal velocity)
            {
            if (direction == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(direction), "Cannot be None.");
            if (velocity <= 0)
                throw new ArgumentOutOfRangeException(nameof(velocity), "Must be positive.");

            this.Direction = direction;
            this.Velocity = velocity;
            this._movingTowards = movingTowards;
            }
            
        public Vector2 MovingTowards
            {
            get
                {
                if (!this._movingTowards.HasValue)
                    throw new InvalidOperationException("Not moving.");
                return _movingTowards.Value;
                }
            }
            
        public bool IsMoving
            {
            get
                {
                var result = this.Direction != Direction.None && this.Velocity != 0 && this._movingTowards.HasValue;
                return result;
                }
            }
        }
    }
