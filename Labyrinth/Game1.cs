using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        public static int Ticks { get; private set; }

        public Game1([NotNull] IServiceSetup services)
            {
            services.Setup(this);
            GlobalServices.SetGame(this);

            this.Content.RootDirectory = "Content";

            // Create the screen manager component.
            var screenManager = new ScreenManager(this);
            screenManager.TraceEnabled = true;
            this.Components.Add(screenManager);

            var gameStartParameters = new GameStartParameters { CountOfLives = 3, World = "World1", WorldLoader = new WorldLoader()};
            var gamePlayScreen = new GameplayScreen(gameStartParameters, screenManager.InputState);
            LoadingScreen.Load(screenManager, true, null, gamePlayScreen);

            Components.Add(new FrameRateCounter(this));
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
        }
    }
