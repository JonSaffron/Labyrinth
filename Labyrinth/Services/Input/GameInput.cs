using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    public class GameInput : GameComponent
        {
        public bool HasGameExitBeenTriggered { get; private set; }
        public bool HasToggleFullScreenBeenTriggered { get; private set; }
        public bool HasSoundOffBeenTriggered { get; private set; }
        public bool HasSoundOnBeenTriggered { get; private set; }
        public bool HasSoundIncreaseBeenTriggered { get; private set; }
        public bool HasSoundDecreaseBeenTriggered { get; private set; }
        public bool HasMoveToNextLevelBeenTriggered { get; set; }

        private KeyboardState _previousKeyboardState;

        public GameInput(Game1 game) : base(game)
            {
            game.Components.Add(this);
            }

        public KeyboardState GetNewKeyboardState(out KeyboardState previousKeyboardState)
            {
            KeyboardState keyboardState = Keyboard.GetState();
            HasToggleFullScreenBeenTriggered = IsKeyNewlyPressed(keyboardState, this._previousKeyboardState, Keys.Enter) && keyboardState.IsKeyDown(Keys.LeftAlt);
            HasSoundOffBeenTriggered = IsKeyNewlyPressed(keyboardState, this._previousKeyboardState, Keys.Q);
            HasSoundOnBeenTriggered = IsKeyNewlyPressed(keyboardState, this._previousKeyboardState, Keys.S);
            HasSoundIncreaseBeenTriggered = IsKeyNewlyPressed(keyboardState, this._previousKeyboardState, Keys.PageUp);
            HasSoundDecreaseBeenTriggered = IsKeyNewlyPressed(keyboardState, this._previousKeyboardState, Keys.PageDown);
            previousKeyboardState = this._previousKeyboardState;
            this._previousKeyboardState = keyboardState;
            return keyboardState;
            }

        public override void Initialize()
            {
            this._previousKeyboardState = Keyboard.GetState();
            }

        public override void Update(GameTime gameTime)
            {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.HasGameExitBeenTriggered = true;

            HasToggleFullScreenBeenTriggered = false;
            HasSoundOffBeenTriggered = false;
            HasSoundOnBeenTriggered = false;
            HasSoundIncreaseBeenTriggered = false;
            HasSoundDecreaseBeenTriggered = false;
            HasMoveToNextLevelBeenTriggered = false;
            }

        public static bool IsKeyNewlyPressed(KeyboardState newState, KeyboardState previousState, Keys key)
            {
            bool result = previousState.IsKeyUp(key) && newState.IsKeyDown(key);
            return result;
            }
        }
    }
