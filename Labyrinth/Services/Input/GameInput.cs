using System;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    public class GameInput
        {
        public bool HasToggleFullScreenBeenTriggered { get; private set; }
        public bool HasIncreaseZoomBeenTriggered { get; private set; }
        public bool HasDecreaseZoomBeenTriggered { get; private set; }
        public bool HasSoundOffBeenTriggered { get; private set; }
        public bool HasSoundOnBeenTriggered { get; private set; }
        public bool HasSoundIncreaseBeenTriggered { get; private set; }
        public bool HasSoundDecreaseBeenTriggered { get; private set; }
        public bool HasMoveToNextLevelBeenTriggered { get; private set; }
        public bool HasPauseBeenTriggered { get; private set; }
        public bool HasGameExitBeenTriggered { get; private set; }

        private readonly InputState _inputState;

        public GameInput(InputState inputState)
            {
            this._inputState = inputState ?? throw new ArgumentNullException(nameof(inputState));
            }

        public void Update()
            {
            this.HasToggleFullScreenBeenTriggered = this._inputState.IsKeyCurrentlyPressed(Keys.LeftAlt) && this._inputState.IsKeyNewlyPressed(Keys.Enter);
            this.HasIncreaseZoomBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.OemPlus);
            this.HasDecreaseZoomBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.OemMinus);
            this.HasSoundOffBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.Q);
            this.HasSoundOnBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.S);
            this.HasSoundIncreaseBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.PageUp);
            this.HasSoundDecreaseBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.PageDown);
            this.HasMoveToNextLevelBeenTriggered = this._inputState.IsKeyCurrentlyPressed(Keys.LeftShift) && this._inputState.IsKeyNewlyPressed(Keys.L);
            this.HasPauseBeenTriggered = this._inputState.IsKeyNewlyPressed(Keys.P);
            this.HasGameExitBeenTriggered = this._inputState.IsNewButtonPress(Buttons.Back, null, out _);
            }
        }
    }
