using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Labyrinth.Services.Display;

namespace Labyrinth.Screens
    {
    internal class MenuEntry
        {
        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        private string _text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
            {
            get => _text;
            set => _text = value ?? throw new ArgumentNullException(nameof(value));
            }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
            {
            get => _position;
            set => _position = value;
            }

        /// <summary>
        /// Gets or sets whether the menu entry is enabled for selection
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs>? Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
            {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
            }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
            {
            this._text = text;
            }   

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
            {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
            }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
            {
            // Draw the selected entry in yellow, otherwise white.
            Color color = IsEnabled ? (isSelected ? Color.Gold : Color.White) : Color.Gray;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = IsEnabled ? (float)Math.Sin(time * 6) + 1 : 0;

            float scale = 1 + pulsate * 0.05f * _selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered in the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            ISpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2.0f);

            spriteBatch.DrawString(font, _text, _position, color, 0, origin, scale, SpriteEffects.None, 0);
            }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
            {
            return screen.ScreenManager.Font.LineSpacing;
            }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
            {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
            }
        }
    }
