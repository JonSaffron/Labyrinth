using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.Services.Display;
using Labyrinth.Services.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    internal class LoadingScreen : GameScreen
        {
        private readonly bool _loadingIsSlow;
        private bool _otherScreensAreGone;

        private readonly List<GameScreen> _screensToLoad;

        private string? _loadingMessage;

        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(bool loadingIsSlow, IEnumerable<GameScreen> screensToLoad)
            {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad.WhereNotNull().ToList();
            if (_screensToLoad.Count == 0)
                throw new ArgumentException("Nothing specified to load", nameof(screensToLoad));

            // we don't serialize loading screens. if the user exits while the
            // game is at a loading screen, the game will resume at the screen
            // before the loading screen.
            IsSerializable = false;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);

            Messenger.Default.Register<WorldLoaderProgress>(this, UpdateLoadingScreenMessage);
            }

        private void UpdateLoadingScreenMessage(WorldLoaderProgress msg)
            {
            _loadingMessage = msg.Message;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            Draw(new GameTime());
            ScreenManager.GraphicsDevice.Present();
            }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
            {
            if (screenManager == null)
                throw new ArgumentNullException(nameof(screenManager));

            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.Screens)
                {
                screen.ExitScreen();
                }

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
            }

        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (_otherScreensAreGone)
                {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                    {
                    ScreenManager.AddScreen(screen, ControllingPlayer);
                    }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();

                Messenger.Default.Unregister<WorldLoaderProgress>(this);
                }
            }

        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if (ScreenState == ScreenState.Active && ScreenManager.Screens.Count() == 1)
                {
                _otherScreensAreGone = true;
                }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (_loadingIsSlow)
                {
                ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                const string message = "Loading...";

                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                int y = viewport.Height / 3;
                Color color = Color.White * TransitionAlpha;

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawCentredString(font, message, y, color);

                if (_loadingMessage != null)
                    spriteBatch.DrawCentredString(font, _loadingMessage, y + 75, color);

                spriteBatch.End();
                }
            }
        }
    }
