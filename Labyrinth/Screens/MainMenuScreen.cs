using Labyrinth.DataStructures;
using Labyrinth.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    internal class MainMenuScreen : MenuScreen
        {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base("Main Menu")
            {
            var world = PersistentStorage.GetWorld();

            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry($"Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            if (world != null)
                {
                MenuEntries.Add(playGameMenuEntry);
                }

            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
            }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        private void PlayGameMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
            {
            var worldToLoad = PersistentStorage.GetWorld();
            if (worldToLoad == null)
                {
                return;
                }
            var worldStartParameters = new WorldStartParameters(countOfLives: 3, worldToLoad: worldToLoad);
            var gamePlayScreen = new GameplayScreen(worldStartParameters, ScreenManager.InputState);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, gamePlayScreen);
            }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void OptionsMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
            {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
            {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
            }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        private void ConfirmExitMessageBoxAccepted(object? sender, PlayerIndexEventArgs _)
            {
            ScreenManager.Game.Exit();
            }

        public override void Draw(GameTime gameTime)
            {
            base.Draw(gameTime);

            string? world = PersistentStorage.GetWorld();
            string worldName = "- No world selected -";
            if (world != null)
                {
                var worldInfo = WorldManagement.GetWorldInfo(world);
                if (worldInfo != null)
                    {
                    worldName = worldInfo.Name;
                    }
                }

            Vector2 textSize = ScreenManager.Font.MeasureString(worldName);
            Vector2 titleOrigin = textSize / 2;
            Color titleColor = Color.Aqua;
            float titleScale = 1f;
            var y = ScreenManager.GraphicsDevice.Viewport.Height - 40f - textSize.Y;
            var textPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, y);

            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, worldName, textPosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
            }
        }
    }
