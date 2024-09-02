using Labyrinth.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Screens
    {
    internal class GameOverScreen : MenuScreen
        {
        public GameOverScreen(string menuTitle) : base(menuTitle)
            {
            // Create our menu entries.
            var scoreEntry = new MenuEntry($"Score: {GlobalServices.ScoreKeeper.CurrentScore:N0}");
            scoreEntry.IsEnabled = false;
            MenuEntry mainMenuEntry = new MenuEntry("Back to Main Menu");

            // Hook up menu event handlers.
            mainMenuEntry.Selected += MainMenuSelected;

            // Add entries to the menu.
            MenuEntries.Add(scoreEntry);
            MenuEntries.Add(mainMenuEntry);
            }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void MainMenuSelected(object? sender, PlayerIndexEventArgs e)
            {
            this.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
            }

        public override void Draw(GameTime gameTime)
            {
            base.Draw(gameTime);

            string? world = PersistentStorage.GetWorld();
            string text = "- No world selected -";
            if (world != null)
                {
                var worldInfo = WorldManagement.GetWorldInfo(world);
                if (worldInfo != null)
                    {
                    text = worldInfo.Name;
                    }
                }

            Vector2 textSize = ScreenManager.Font.MeasureString(text);
            Vector2 titleOrigin = textSize / 2;
            Color titleColor = Color.Aqua;
            float titleScale = 1f;
            var y = ScreenManager.GraphicsDevice.Viewport.Height - 50f - textSize.Y;
            var textPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, y);

            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.Font, text, textPosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
            }

        protected override void OnCancel(PlayerIndex playerIndex)
            {
            MainMenuSelected(this, new PlayerIndexEventArgs(playerIndex));
            }
        }
    }
