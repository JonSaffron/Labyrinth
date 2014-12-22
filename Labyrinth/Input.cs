using Microsoft.Xna.Framework.Input;

namespace Labyrinth
    {
    static class Input
        {
        private static KeyboardState _previousKeyboardState;
        private static Direction _previousRequestedDirectionOfMovement;
        
        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        public static Direction GetRequestedDirectionOfMovement(out bool isFiring, out bool moveToNextLevel, out bool toggleFullScreen)
            {
            KeyboardState keyboardState = Keyboard.GetState();

            isFiring = IsKeyNewlyPressed(keyboardState, Keys.LeftControl);
            moveToNextLevel = IsKeyNewlyPressed(keyboardState, Keys.L) && keyboardState.IsKeyDown(Keys.LeftShift);
            toggleFullScreen = IsKeyNewlyPressed(keyboardState, Keys.Enter) && keyboardState.IsKeyDown(Keys.LeftAlt);

            var result = GetNewDirection(keyboardState);
            if (result == Direction.None)
                {
                result = GetContinuedDirection(keyboardState);
                }
            if (result == Direction.None)
                {
                result = GetRemnantDirection(keyboardState);
                }
            
            _previousKeyboardState = keyboardState;
            _previousRequestedDirectionOfMovement = result;
            
            return result;
            }

        private static Direction GetRemnantDirection(KeyboardState keyboardState)
            {
            Direction result = Direction.None;
            switch (_previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                case Direction.Right:
                    if (keyboardState.IsKeyDown(Keys.Up) && !keyboardState.IsKeyDown(Keys.Down))
                        result = Direction.Up;
                    else if (keyboardState.IsKeyDown(Keys.Down) && !keyboardState.IsKeyDown(Keys.Up))
                        result = Direction.Down;
                    break;
                case Direction.Up:
                case Direction.Down:
                    if (keyboardState.IsKeyDown(Keys.Left) && !keyboardState.IsKeyDown(Keys.Right))
                        result = Direction.Left;
                    else if (keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
                        result = Direction.Right;
                    break;
                }
            return result;
            }

        private static Direction GetContinuedDirection(KeyboardState keyboardState)
            {
            Direction result = Direction.None;
            switch (_previousRequestedDirectionOfMovement)
                {
                case Direction.Left:
                    if (keyboardState.IsKeyDown(Keys.Left))
                        result = Direction.Left;
                    break;

                case Direction.Right:
                    if (keyboardState.IsKeyDown(Keys.Right))
                        result = Direction.Right;
                    break;

                case Direction.Up:
                    if (keyboardState.IsKeyDown(Keys.Up))
                        result = Direction.Up;
                    break;

                case Direction.Down:
                    if (keyboardState.IsKeyDown(Keys.Down))
                        result = Direction.Down;
                    break;
                }
            return result;
            }

        private static Direction GetNewDirection(KeyboardState keyboardState)
            {
            Direction result = Direction.None;
            if (IsKeyNewlyPressed(keyboardState, Keys.Left))
                result = Direction.Left;
            if (IsKeyNewlyPressed(keyboardState, Keys.Up))
                result = Direction.Up;
            if (IsKeyNewlyPressed(keyboardState, Keys.Right))
                result = Direction.Right;
            if (IsKeyNewlyPressed(keyboardState, Keys.Down))
                result = Direction.Down;
            return result;
            }

        private static bool IsKeyNewlyPressed(KeyboardState newState, Keys key)
            {
            bool result = _previousKeyboardState.IsKeyUp(key) && newState.IsKeyDown(key);
            return result;
            }
        }
    }
