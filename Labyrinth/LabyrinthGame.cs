using Labyrinth.Screens;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for the game
    /// </summary>
    public class LabyrinthGame : Game, ISpriteLibrary
        {
        public LabyrinthGame()
            {
            var services = new InteractiveServices();
            services.Setup(this);

            this.Content.RootDirectory = "Content";

            // Create the screen manager component.
            var screenManager = new ScreenManager(this);
            this.Components.Add(screenManager);
#if DEBUG
            this.Components.Add(new FrameRateCounter(this));
            screenManager.TraceEnabled = true;
#endif

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
            }

        public Texture2D GetSprite(string textureName)
            {
            if (textureName == null)
                throw new ArgumentNullException(nameof(textureName));
            if (string.IsNullOrWhiteSpace(textureName))
                throw new ArgumentException("Invalid texture name.", nameof(textureName));

            var result = this.Content.Load<Texture2D>(textureName);
            return result;
            }
        }
    }
