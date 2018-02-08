using System;
using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        private readonly GraphicsDeviceManager _gdm;
        [NotNull] private readonly IWorldLoader _worldLoader;
        [NotNull] private readonly IScoreKeeper _scoreKeeper;
        private readonly IHeadsUpDisplay _headsUpDisplay = new HeadsUpDisplay();
        private ISpriteBatch _spriteBatch;

        private World _world;
        private int _lives;
        private bool _isGamePaused;
        public static int Ticks { get; private set; }

        public Game1([NotNull] IServiceSetup services)
            {
            services.Setup(this);
            this._worldLoader = services.WorldLoader ?? throw new ArgumentException("WorldLoader");
            this._scoreKeeper = services.ScoreKeeper ?? throw new ArgumentException("ScoreKeeper");
            GlobalServices.SetGame(this);
            
            this._gdm = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferWidth = (int) Constants.RoomSizeInPixels.X * Constants.ZoomWhilstWindowed,
                                PreferredBackBufferHeight = (int) Constants.RoomSizeInPixels.Y * Constants.ZoomWhilstWindowed
                            };

            this.Content.RootDirectory = "Content";
            this._lives = 2;
            this._headsUpDisplay.Reset();
            this._scoreKeeper.Reset();
            }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
            {
            // Create a new SpriteBatch, which can be used to draw textures.
            this._spriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);

            GlobalServices.SoundPlayer.SoundLibrary.LoadContent(this.Content);
            this._headsUpDisplay.LoadContent(this.Content);
            }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
            {
            if (this._world == null) 
                return;
            
            this._spriteBatch.Dispose();
            this.World = null;
            }

        private World World
            {
            set
                {
                this._world = value;
                GlobalServices.SetWorld(value);
                }
            }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update</param>
        protected override void Update(GameTime gameTime)
            {
            if (this._world == null)
                {
                LoadLevel("World1.xml");
                }

            // Must process keyboard input before checking pause status
            ProcessGameInput();
            if (this._isGamePaused)
                gameTime = new GameTime();

            // allow registered components to update. this includes the GameTimers and the Keyboard handler.
            Ticks += 1;
            base.Update(gameTime);
            GlobalServices.SoundPlayer.ActiveSoundService.Update();

            if (!this._isGamePaused)
                {
                // ReSharper disable once PossibleNullReferenceException
                LevelReturnType lrt = this._world.Update(gameTime);
                switch (lrt)
                    {
                    case LevelReturnType.FinishedWorld:
                        this.World = null;
                        this._lives++;
                        break;
                
                    case LevelReturnType.LostLife:
                        GlobalServices.SoundPlayer.ActiveSoundService.Clear();
                        if (this._lives == 0)
                            {
                            this.Exit();
                            return;
                            }
                        this._lives--;
                        this._world.ResetLevelAfterLosingLife();
                        break;
                    }
                }

            if (gameTime.IsRunningSlowly)
                {
                string text = string.Format("{0}: Running slowly", gameTime.TotalGameTime);
                System.Diagnostics.Trace.WriteLine(text);
                }
            }

        private void ProcessGameInput()
            {
            var gameInput = GlobalServices.GameInput;
            if (gameInput.HasGameExitBeenTriggered)
                this.Exit();

            if (gameInput.HasPauseBeenTriggered)
                this._isGamePaused = !this._isGamePaused;

            if (gameInput.HasToggleFullScreenBeenTriggered)
                ToggleFullScreen();
                
            int changeToEnabled = (gameInput.HasSoundOnBeenTriggered ? 1 : 0) + (gameInput.HasSoundOffBeenTriggered ? -1 : 0);
            if (changeToEnabled < 0)
                GlobalServices.SoundPlayer.Mute();
            else if (changeToEnabled > 0)
                GlobalServices.SoundPlayer.Unmute();

            int changeToVolume = (gameInput.HasSoundIncreaseBeenTriggered ? 1 : 0) + (gameInput.HasSoundDecreaseBeenTriggered  ? -1 : 0);
            if (changeToVolume < 0)
                GlobalServices.SoundPlayer.TurnDownTheVolume();
            else if (changeToVolume > 0)
                GlobalServices.SoundPlayer.TurnUpTheVolume();

            if (gameInput.HasMoveToNextLevelBeenTriggered)
                {
                this._world?.MoveUpALevel();
                }
            }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        protected override void Draw(GameTime gameTime)
            {
            GraphicsDevice.Clear(Color.Black);
            
            if (this._isGamePaused)
                gameTime = new GameTime();

            // Draw the sprite.
            if (this._world != null)
                {
                this._spriteBatch.Begin(this._world.WorldWindow.WindowPosition);

                this._world.Draw(gameTime, _spriteBatch);
                this._headsUpDisplay.DrawStatus(_spriteBatch, GlobalServices.GameState.Player.IsExtant, GlobalServices.GameState.Player.Energy, this._scoreKeeper.CurrentScore, this._lives, this._isGamePaused, gameTime.IsRunningSlowly);
                
                base.Draw(gameTime);
                this._spriteBatch.End();
                }
            }
            
        internal void LoadLevel(string level)
            {
            // Load the World.
            this.World = new World(this._worldLoader, level);
            this._world.ResetLevelForStartingNewLife();
            }

        private void ToggleFullScreen()
            {
            this._gdm.ToggleFullScreen();
            this._spriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);
            }

        private static ISpriteBatch GetSpriteBatch(GraphicsDevice graphicsDevice, bool isFullScreen)
            {
            var result = isFullScreen ? (ISpriteBatch) new SpriteBatchFullScreen(graphicsDevice) : new SpriteBatchWindowed(graphicsDevice, Constants.ZoomWhilstWindowed);
            return result;
            }

        /// <inheritdoc />
        protected override void OnDeactivated(object sender, EventArgs args)
            {
            base.OnDeactivated(sender, args);

            this._isGamePaused = true;
            }
        }
    }
