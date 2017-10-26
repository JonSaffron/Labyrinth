using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Labyrinth.Services.ScoreKeeper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        private readonly IPlayerInput _playerInput;
        private ISpriteBatch _spriteBatch;

        private readonly GraphicsDeviceManager _gdm;
        private readonly IWorldLoader _worldLoader;

        private int _displayedScore;
        private int _lives;

        private readonly IHeadsUpDisplay _headsUpDisplay = new HeadsUpDisplay();
        private readonly IScoreKeeper _scoreKeeper = new ScoreKeeper();
        private World _world;

        private bool GameIsPaused { get; set; }

        public Game1(IPlayerInput playerInput, IWorldLoader worldLoader, ISoundPlayer soundPlayer, ISpriteLibrary spriteLibrary)
            {
            if (playerInput == null)
                throw new ArgumentNullException("playerInput");
            if (worldLoader == null)
                throw new ArgumentNullException("worldLoader");
            this._playerInput = playerInput;
            this._playerInput.GameInput = new GameInput(this);
            this._worldLoader = worldLoader;
            GlobalServices.SetSoundPlayer(soundPlayer);
            GlobalServices.SetSpriteLibrary(spriteLibrary);
            GlobalServices.SetServiceProvider(this.Services);
            GlobalServices.SetGameComponentCollection(this.Components);
            GlobalServices.SetPlayerInput(this._playerInput);
            GlobalServices.SetScoreKeeper(this._scoreKeeper);

            this._gdm = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferWidth = (int) Constants.RoomSizeInPixels.X * Constants.ZoomWhilstWindowed,
                                PreferredBackBufferHeight = (int) Constants.RoomSizeInPixels.Y * Constants.ZoomWhilstWindowed
                            };

            this.Content.RootDirectory = "Content";
            //this.TargetElapsedTime = new TimeSpan(this.TargetElapsedTime.Ticks * 4);
            this._lives = 2;
            this._scoreKeeper.Reset();
            this._displayedScore = 0;
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
            if (this.World == null) 
                return;
            
            this._spriteBatch.Dispose();
            this.World = null;
            }

        internal World World
            {
            get
                {
                return _world;
                }

            private set
                {
                _world = value;
                GlobalServices.SetCentrePointProvider(value);
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
            // todo IsActive returns false during unit testing. Not useful.
#if !DEBUG
            if (!this.IsActive)
                return;
#endif

            if (this.World == null)
                {
                LoadLevel("World1.xml");
                }

            base.Update(gameTime);

            if (!this.GameIsPaused)
                {
                // ReSharper disable once PossibleNullReferenceException
                LevelReturnType lrt = this.World.Update(gameTime);
                switch (lrt)
                    {
                    case LevelReturnType.FinishedLevel:
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
                        this.World.ResetLevelAfterLosingLife(_spriteBatch);
                        break;
                    }
                }

            if (gameTime.IsRunningSlowly)
                {
                string text = string.Format("{0}: Running slowly", gameTime.TotalGameTime);
                System.Diagnostics.Trace.WriteLine(text);
                }

            if (this._displayedScore < this._scoreKeeper.CurrentScore)
                this._displayedScore++;

            ProcessGameInput();
            GlobalServices.SoundPlayer.ActiveSoundService.Update();
            }

        private void ProcessGameInput()
            {
            var gameInput = this._playerInput.GameInput;
            if (gameInput.HasGameExitBeenTriggered)
                this.Exit();

            if (gameInput.HasPauseBeenTriggered)
                this.GameIsPaused = !this.GameIsPaused;

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

            if (gameInput.HasMoveToNextLevelBeenTriggered && this.World != null)
                {
                this.World.MoveUpALevel();
                }
            }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        protected override void Draw(GameTime gameTime)
            {
            GraphicsDevice.Clear(Color.Black);

            // Draw the sprite.
            _spriteBatch.Begin(this.World.WindowPosition);
            if (this.World != null)
                {
                this.World.Draw(gameTime, _spriteBatch);
                this._headsUpDisplay.DrawStatus(_spriteBatch, this.World.Player.IsExtant, this.World.Player.Energy, this._displayedScore, this._lives);
                if (this.GameIsPaused)
                    this._headsUpDisplay.DrawPausedMessage(_spriteBatch);
                }

            _spriteBatch.End();

            base.Draw(gameTime);
            }
            
        internal void LoadLevel(string level)
            {
            // Load the World.
            this._worldLoader.LoadWorld(level);
            this.World = new World(this._worldLoader);
            this.World.ResetLevelForStartingNewLife(this._spriteBatch);
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

        protected override void OnDeactivated(object sender, EventArgs args)
            {
            base.OnDeactivated(sender, args);

            this.GameIsPaused = true;
            }
        }
    }
