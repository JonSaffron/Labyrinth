using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Labyrinth.Services.Input;
using Microsoft.Xna.Framework.Content;
using Labyrinth.Services.Display;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    internal abstract class MenuScreen : GameScreen
        {
        private readonly string _menuTitle;
        private readonly List<MenuEntry> _menuEntries = new List<MenuEntry>();
        private int? _selectedEntry;

        private readonly InputAction _menuUp;
        private readonly InputAction _menuDown;
        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        private ContentManager? _content;
        private Texture2D? _titleTexture;
        private Rectangle _titleRect;
        private Vector2 _titlePosition;

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries => _menuEntries;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected MenuScreen(string menuTitle)
            {
            _menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _menuUp = new InputAction(
                new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new[] { Keys.Up },
                true);
            _menuDown = new InputAction(
                new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new[] { Keys.Down },
                true);
            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space },
                true);
            _menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Escape },
                true);
            }

        public override void Activate(bool instancePreserved = false)
            {
            if (!instancePreserved)
                {
                if (_content == null)
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");

                _titleTexture = _content.Load<Texture2D>(@"Display\LabyrinthTitle");
                }

            this._selectedEntry = Enumerable.Range(0, this.MenuEntries.Count)
                .FirstOrDefault(i => this.MenuEntries[i].IsEnabled);
            }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
            {
            _content?.Unload();
            }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
            {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            int countOfEntries = this.MenuEntries.Count;
            int selectedEntry = this._selectedEntry.GetValueOrDefault(0);
            // Move to the previous menu entry?
            if (_menuUp.Evaluate(ScreenManager.InputState, ControllingPlayer, out _))
                {
                this._selectedEntry = Enumerable.Range(1, countOfEntries)
                    .Select(i => (selectedEntry + countOfEntries - i) % countOfEntries)
                    .FirstOrDefault(i => this.MenuEntries[i].IsEnabled);
                }

            // Move to the next menu entry?
            if (_menuDown.Evaluate(ScreenManager.InputState, ControllingPlayer, out _))
                {
                this._selectedEntry = Enumerable.Range(1, countOfEntries)
                    .Select(i => (selectedEntry + i) % countOfEntries)
                    .FirstOrDefault(i => this.MenuEntries[i].IsEnabled);
                }

            if (_menuSelect.Evaluate(ScreenManager.InputState, ControllingPlayer, out playerIndex))
                {
                if (this._selectedEntry.HasValue && this.MenuEntries[this._selectedEntry.Value].IsEnabled)
                    {
                    OnSelectEntry(_selectedEntry.Value, playerIndex);
                    }
                }
            else if (_menuCancel.Evaluate(ScreenManager.InputState, ControllingPlayer, out playerIndex))
                {
                OnCancel(playerIndex);
                }
            }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
            {
            _menuEntries[entryIndex].OnSelectEntry(playerIndex);
            }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
            {
            ExitScreen();
            }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object? sender, PlayerIndexEventArgs e)
            {
            OnCancel(e.PlayerIndex);
            }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default,
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations(float zoom)
            {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            var y = _titleRect.Bottom + 100f - transitionOffset * 100f;
            this._titlePosition = new Vector2(graphics.Viewport.Width / 2f, y);

            // start below menu title; each X value is generated per entry
            y += ScreenManager.Font.LineSpacing * 1.25f * zoom + 50f;

            // update each menu entry's location in turn
            foreach (var menuEntry in _menuEntries)
                {
                // each entry is to be centered horizontally
                float x = ScreenManager.GraphicsDevice.Viewport.Width / 2f - menuEntry.GetWidth(this) / 2f * zoom;

                if (ScreenState == ScreenState.TransitionOn)
                    x -= transitionOffset * 256;
                else
                    x += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = new Vector2(x, y);

                // move down for the next entry the size of this entry
                y += menuEntry.GetHeight(this) * zoom;
                }
            }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuEntries.Count; i++)
                {
                bool isSelected = IsActive && i == _selectedEntry;

                _menuEntries[i].Update(this, isSelected, gameTime);
                }
            }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations(spriteBatch.Zoom);

            spriteBatch.Begin();

            if (_titleTexture != null)
                {
                var graphics = ScreenManager.GraphicsDevice;
                int x = graphics.Viewport.Width / 10 * 2;
                int y = 15;
                int w = graphics.Viewport.Width / 10 * 6;
                float scale = w / (float)_titleTexture.Width;
                int h = (int)(_titleTexture.Height * scale);
                _titleRect = new Rectangle(x, y, w, h);
                spriteBatch.DrawTextureOverRegion(_titleTexture, _titleRect, Color.Aquamarine);
                }

            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; i++)
                {
                MenuEntry menuEntry = _menuEntries[i];

                bool isSelected = IsActive && i == _selectedEntry;

                menuEntry.Draw(this, isSelected, gameTime);
                }

            Vector2 titleOrigin = font.MeasureString(_menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            spriteBatch.DrawString(font, _menuTitle, this._titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
            }
        }
    }
