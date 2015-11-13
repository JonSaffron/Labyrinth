using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public struct Movement
        {
        private static readonly Movement StandingStillMovement;

        private readonly Direction _direction;
        private readonly Vector2? _movingTowards;
        private readonly decimal _velocity;

        static Movement()
            {
            StandingStillMovement = new Movement();
            }

        public Movement(Direction direction, Vector2 movingTowards, decimal velocity)
            {
            if (direction == Direction.None)
                throw new ArgumentOutOfRangeException("direction", "Cannot be None.");
            if (velocity <= 0)
                throw new ArgumentOutOfRangeException("velocity", "Must be positive.");

            this._direction = direction;
            this._movingTowards = movingTowards;
            this._velocity = velocity;
            }

        public static Movement Still
            {
            get
                {
                return StandingStillMovement;
                }
            }

        public Direction Direction
            {
            get
                {
                return _direction;
                }
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

        public decimal Velocity
            {
            get
                {
                return _velocity;
                }
            }

        public bool IsMoving
            {
            get
                {
                var result = this._direction != Direction.None && this._velocity != 0 && this._movingTowards.HasValue;
                return result;
                }
            }
        }
    }
