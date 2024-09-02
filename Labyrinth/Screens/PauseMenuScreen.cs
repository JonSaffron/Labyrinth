
using Microsoft.Xna.Framework;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    internal class PauseMenuScreen : MenuScreen
        {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen() : base("Paused")
            {
            // prevents the underlying GameplayScreen from being hidden
            this.IsPopup = true;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
        
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
            }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        private void QuitGameMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
            {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
            }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        private void ConfirmQuitMessageBoxAccepted(object? sender, PlayerIndexEventArgs e)
            {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
            }

        public override void Draw(GameTime gameTime)
            {
            var r = this.ScreenManager.GraphicsDevice.Viewport.Bounds;
            var spriteBatch = this.ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(r, Color.Black * (this.TransitionAlpha / 3f));
            spriteBatch.End();

            base.Draw(gameTime);
            }
        }
    }
