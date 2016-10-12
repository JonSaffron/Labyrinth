using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    class PlayerInput : IPlayerInput
        {
        public GameInput GameInput { get; set; }
        public Direction Direction { get; private set; }
        public FiringState FireStatus1 { get; private set; }
        public FiringState FireStatus2 { get; private set; }

        private Direction _previousRequestedDirectionOfMovement;
        
        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        public void ProcessInput(GameTime gameTime)
            {
            var direction = GetNewDirection();
            if (direction == Direction.None)
                {
                direction = GetContinuedDirection();
                if (direction == Direction.None)
                    {
                    direction = GetRemnantDirection();
                    }
                }
            this.Direction = direction;
            
            this.FireStatus1 = this.GameInput.IsKeyCurrentlyPressed(Keys.LeftControl) ? (this.GameInput.WasKeyPressed(Keys.LeftControl) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;
            this.FireStatus2 = this.GameInput.IsKeyCurrentlyPressed(Keys.Space) ? (this.GameInput.WasKeyPressed(Keys.Space) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;

            this._previousRequestedDirectionOfMovement = direction;
            }

        private Direction GetNewDirection()
            {
            if (this.GameInput.IsKeyNewlyPressed(Keys.Left))
                return Direction.Left;
            if (this.GameInput.IsKeyNewlyPressed(Keys.Up))
                return Direction.Up;
            if (this.GameInput.IsKeyNewlyPressed(Keys.Right))
                return Direction.Right;
            if (this.GameInput.IsKeyNewlyPressed(Keys.Down))
                return Direction.Down;
            return Direction.None;
            }

        private Direction GetContinuedDirection()
            {
            switch (this._previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Left))
                        return Direction.Left;
                    break;

                case Direction.Right:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Right))
                        return Direction.Right;
                    break;

                case Direction.Up:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Up))
                        return Direction.Up;
                    break;

                case Direction.Down:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Down))
                        return Direction.Down;
                    break;
                }
            return Direction.None;
            }

        private Direction GetRemnantDirection()
            {
            switch (this._previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                case Direction.Right:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Up) && !this.GameInput.IsKeyCurrentlyPressed(Keys.Down))
                        return Direction.Up;
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Down) && !this.GameInput.IsKeyCurrentlyPressed(Keys.Up))
                        return Direction.Down;
                    break;
                case Direction.Up:
                case Direction.Down:
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Left) && !this.GameInput.IsKeyCurrentlyPressed(Keys.Right))
                        return Direction.Left;
                    if (this.GameInput.IsKeyCurrentlyPressed(Keys.Right) && !this.GameInput.IsKeyCurrentlyPressed(Keys.Left))
                        return Direction.Right;
                    break;
                }
            return Direction.None;
            }
        }
    }
