using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Labyrinth.Services.Input
    {
    public class InputState
        {
        public const int MaxInputs = 4;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public readonly GamePadState[] CurrentGamePadStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
            {
            this._currentKeyboardState = Keyboard.GetState();

            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            
            GamePadWasConnected = new bool[MaxInputs];
            }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
            {
            this._previousKeyboardState = this._currentKeyboardState;
            this._currentKeyboardState = Keyboard.GetState();
            
            for (int i = 0; i < MaxInputs; i++)
                {
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex) i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                    {
                    GamePadWasConnected[i] = true;
                    }
                }

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
                {
                Gestures.Add(TouchPanel.ReadGesture());
                }
            }
            
        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        /// <param name="key">The key to examine</param>
        public bool IsKeyNewlyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyUp(key) && this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        /// <summary>
        /// Helper for checking if a key is currently pressed during this update
        /// </summary>
        /// <param name="key">The key to examine</param>
        /// <returns></returns>
        public bool IsKeyCurrentlyPressed(Keys key)
            {
            bool result = this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        /// <summary>
        /// Helper for checking if a key was previously pressed in the last update
        /// </summary>
        /// <param name="key">The key to examine</param>
        /// <returns></returns>
        public bool WasKeyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyDown(key);
            return result;
            }

        /// <summary>
        /// Helper for checking if a button was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
            {
            if (controllingPlayer.HasValue)
                {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
                }

            // Accept input from any player.
            return (IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                    IsButtonPressed(button, PlayerIndex.Four, out playerIndex));
            }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
            {
            if (controllingPlayer.HasValue)
                {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
                }

            // Accept input from any player.
            return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }
    }
