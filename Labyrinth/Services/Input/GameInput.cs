using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    public class GameInput : GameComponent
        {
        public bool HasToggleFullScreenBeenTriggered { get; private set; }
        public bool HasSoundOffBeenTriggered { get; private set; }
        public bool HasSoundOnBeenTriggered { get; private set; }
        public bool HasSoundIncreaseBeenTriggered { get; private set; }
        public bool HasSoundDecreaseBeenTriggered { get; private set; }
        public bool HasMoveToNextLevelBeenTriggered { get; private set; }
        public bool HasPauseBeenTriggered { get; private set; }
        public bool HasGameExitBeenTriggered { get; private set; }

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public GameInput(Game1 game) : base(game)
            {
            game.Components.Add(this);
            }

        public override void Initialize()
            {
            this._currentKeyboardState = Keyboard.GetState();
            }

        public override void Update(GameTime gameTime)
            {
            this._previousKeyboardState = this._currentKeyboardState;
            this._currentKeyboardState = Keyboard.GetState();

            this.HasToggleFullScreenBeenTriggered = IsKeyNewlyPressed(Keys.Enter) && IsKeyCurrentlyPressed(Keys.LeftAlt);
            this.HasSoundOffBeenTriggered = IsKeyNewlyPressed(Keys.Q);
            this.HasSoundOnBeenTriggered = IsKeyNewlyPressed(Keys.S);
            this.HasSoundIncreaseBeenTriggered = IsKeyNewlyPressed(Keys.PageUp);
            this.HasSoundDecreaseBeenTriggered = IsKeyNewlyPressed(Keys.PageDown);
            this.HasMoveToNextLevelBeenTriggered = IsKeyNewlyPressed(Keys.L) && IsKeyCurrentlyPressed(Keys.LeftShift);
            this.HasPauseBeenTriggered = IsKeyNewlyPressed(Keys.P);
            this.HasGameExitBeenTriggered = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
            }

        public bool IsKeyNewlyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyUp(key) && this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        public bool IsKeyCurrentlyPressed(Keys key)
            {
            bool result = this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        public bool WasKeyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyDown(key);
            return result;
            }
        }
    }
