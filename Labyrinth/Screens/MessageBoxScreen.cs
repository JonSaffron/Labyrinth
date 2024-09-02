using Labyrinth.Services.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Labyrinth.Screens
    {
    internal class MessageBoxScreen : GameScreen
        {
        private readonly string _message;
        private Texture2D? _gradientTexture;

        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        public event EventHandler<PlayerIndexEventArgs>? Accepted;
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event EventHandler<PlayerIndexEventArgs>? Cancelled;

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText = true)
            {
            if (message == null) throw new ArgumentNullException(nameof(message));
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Esc = cancel";

            if (includeUsageText)
                _message = message + usageText;
            else
                _message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Space, Keys.Enter },
                true);
            _menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Escape, Keys.Back },
                true);
            }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved = false)
            {
            if (!instancePreserved)
                {
                ContentManager content = ScreenManager.Game.Content;
                _gradientTexture = content.Load<Texture2D>(@"Display\gradient");
                }
            }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput()
            {
            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (_menuSelect.Evaluate(ScreenManager.InputState, ControllingPlayer, out var playerIndex))
                {
                // Raise the accepted event, then exit the message box.
                Accepted?.Invoke(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
                }
            else if (_menuCancel.Evaluate(ScreenManager.InputState, ControllingPlayer, out playerIndex))
                {
                // Raise the cancelled event, then exit the message box.
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
                }
            }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            float zoom = spriteBatch.Zoom;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(_message) * zoom;
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int) (textPosition.X) - hPad,
                                                          (int) (textPosition.Y) - vPad,
                                                          (int) (textSize.X) + hPad * 2,
                                                          (int) (textSize.Y) + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = Color.Cyan * TransitionAlpha;
            
            spriteBatch.Begin();

            // Draw the background rectangle.
            if (this._gradientTexture != null)
                {
                spriteBatch.DrawTextureOverRegion(this._gradientTexture, backgroundRectangle, color);
                }

            // Draw the message box text.
            spriteBatch.DrawCentredString(font, _message, textPosition.Y, color);

            spriteBatch.End();
            }
        }
    }
