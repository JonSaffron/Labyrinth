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
        public Game1()
            {
            var services = new InteractiveServices();
            services.Setup(this);

            this.Content.RootDirectory = "Content";

            // Create the screen manager component.
            var screenManager = new ScreenManager(this);
            this.Components.Add(screenManager);
#if DEBUG
            screenManager.TraceEnabled = true;
#endif

            var gameStartParameters = new GameStartParameters { CountOfLives = 3, WorldToLoad = "World2", WorldLoader = new WorldLoader()};
            var gamePlayScreen = new GameplayScreen(gameStartParameters, screenManager.InputState);
            LoadingScreen.Load(screenManager, true, null, gamePlayScreen);

#if DEBUG
            this.Components.Add(new FrameRateCounter(this));
#endif
            }
        }
    }
