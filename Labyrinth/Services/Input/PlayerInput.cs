using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth
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
            KeyboardState previousKeyboardState;
            KeyboardState keyboardState = this.GameInput.GetNewKeyboardState(out previousKeyboardState);

            var direction = GetNewDirection(keyboardState, previousKeyboardState);
            if (direction == Direction.None)
                {
                direction = GetContinuedDirection(keyboardState);
                if (direction == Direction.None)
                    direction = GetRemnantDirection(keyboardState);
                }
            this.Direction = direction;
            
            if (GameInput.IsKeyNewlyPressed(keyboardState, previousKeyboardState, Keys.L) && keyboardState.IsKeyDown(Keys.LeftShift))
                this.GameInput.HasMoveToNextLevelBeenTriggered = true; 
            this.FireStatus1 = keyboardState.IsKeyDown(Keys.LeftControl) ? (previousKeyboardState.IsKeyDown(Keys.LeftControl) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;
            this.FireStatus2 = keyboardState.IsKeyDown(Keys.Space) ? (previousKeyboardState.IsKeyDown(Keys.Space) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;

            this._previousRequestedDirectionOfMovement = direction;
            }

        private static Direction GetNewDirection(KeyboardState newState, KeyboardState previousState)
            {
            if (GameInput.IsKeyNewlyPressed(newState, previousState, Keys.Left))
                return Direction.Left;
            if (GameInput.IsKeyNewlyPressed(newState, previousState, Keys.Up))
                return Direction.Up;
            if (GameInput.IsKeyNewlyPressed(newState, previousState, Keys.Right))
                return Direction.Right;
            if (GameInput.IsKeyNewlyPressed(newState, previousState, Keys.Down))
                return Direction.Down;
            return Direction.None;
            }

        private Direction GetContinuedDirection(KeyboardState keyboardState)
            {
            switch (this._previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                    if (keyboardState.IsKeyDown(Keys.Left))
                        return Direction.Left;
                    break;

                case Direction.Right:
                    if (keyboardState.IsKeyDown(Keys.Right))
                        return Direction.Right;
                    break;

                case Direction.Up:
                    if (keyboardState.IsKeyDown(Keys.Up))
                        return Direction.Up;
                    break;

                case Direction.Down:
                    if (keyboardState.IsKeyDown(Keys.Down))
                        return Direction.Down;
                    break;
                }
            return Direction.None;
            }

        private Direction GetRemnantDirection(KeyboardState keyboardState)
            {
            switch (this._previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                case Direction.Right:
                    if (keyboardState.IsKeyDown(Keys.Up) && !keyboardState.IsKeyDown(Keys.Down))
                        return Direction.Up;
                    if (keyboardState.IsKeyDown(Keys.Down) && !keyboardState.IsKeyDown(Keys.Up))
                        return Direction.Down;
                    break;
                case Direction.Up:
                case Direction.Down:
                    if (keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
                        return Direction.Left;
                    if (keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
                        return Direction.Right;
                    break;
                }
            return Direction.None;
            }
        }
    }
