using Microsoft.Xna.Framework.Input;

namespace Labyrinth
    {
    static class Input
        {
        private static KeyboardState _previousKeyboardState;
        private static Direction _previousRequestedDirectionOfMovement;
        
        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        public static Direction GetRequestedDirectionOfMovement(out bool isFiring, out bool moveToNextLevel)
            {
            // Get input state.
            //GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            isFiring = IsKeyNewlyPressed(keyboardState, Keys.LeftControl);

            moveToNextLevel = IsKeyNewlyPressed(keyboardState, Keys.L) && keyboardState.IsKeyDown(Keys.LeftShift);

            //// Get analog horizontal movement.
            //movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            //// Ignore small movements to prevent running in place.
            //if (Math.Abs(movement) < 0.5f)
            //    movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            Direction result = Direction.None;
            if (IsKeyNewlyPressed(keyboardState, Keys.Left))
                result = Direction.Left;
            if (IsKeyNewlyPressed(keyboardState, Keys.Up))
                result = Direction.Up;
            if (IsKeyNewlyPressed(keyboardState, Keys.Right))
                result = Direction.Right;
            if (IsKeyNewlyPressed(keyboardState, Keys.Down))
                result = Direction.Down;
            
            if (result == Direction.None)
                {
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
                }
            
            if (result == Direction.None)
                {
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
                }
            
            _previousKeyboardState = keyboardState;
            _previousRequestedDirectionOfMovement = result;
            
            return result;
            }

        private static bool IsKeyNewlyPressed(KeyboardState newState, Keys key)
            {
            bool result = _previousKeyboardState.IsKeyUp(key) && newState.IsKeyDown(key);
            return result;
            }
        }
    }
