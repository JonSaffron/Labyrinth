using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        private readonly ScreenManager _screenManager;

        public static int Ticks { get; private set; }

        public Game1([NotNull] IServiceSetup services)
            {
            services.Setup(this);
            GlobalServices.SetGame(this);

            this.Content.RootDirectory = "Content";
            Components.Add(new FrameRateCounter(this));

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);
            _screenManager.TraceEnabled = true;
            this.Components.Add(this._screenManager);

            var gameStartParameters = new GameStartParameters { CountOfLives = 3, World = "World1" };
            var gamePlayScreen = new GameplayScreen(gameStartParameters, this._screenManager.InputState);
            LoadingScreen.Load(this._screenManager, true, null, gamePlayScreen);
            }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update</param>
        protected override void Update(GameTime gameTime)
            {
            Ticks += 1;
            base.Update(gameTime);
            }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        protected override void Draw(GameTime gameTime)
            {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            }
            
        /// <inheritdoc />
        protected override void OnDeactivated(object sender, EventArgs args)
            {
            base.OnDeactivated(sender, args);

            // todo find another way to do this
            //this._isGamePaused = true;
            }
        }
    }
