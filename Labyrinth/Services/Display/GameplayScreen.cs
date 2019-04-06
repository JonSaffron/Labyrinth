using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Input;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// This screen implements the actual game. 
    /// </summary>
    class GameplayScreen : GameScreen
        {
        private readonly GameStartParameters _gameStartParameters;
        private ContentManager _content;
        private bool _isGamePaused;
        [NotNull] private readonly IWorldLoader _worldLoader;
        [NotNull] private readonly IScoreKeeper _scoreKeeper;
        private readonly IHeadsUpDisplay _headsUpDisplay = new HeadsUpDisplay();
        private readonly GameInput _gameInput;
        private World _world;
        private int _lives;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen([NotNull] GameStartParameters gameStartParameters, InputState inputState)
            {
            this._gameStartParameters = gameStartParameters ?? throw new ArgumentNullException(nameof(gameStartParameters));
            this.TransitionOnTime = TimeSpan.FromSeconds(1.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this._worldLoader = new WorldLoader();
            this._scoreKeeper = new ScoreKeeper.ScoreKeeper();
            this._gameInput = new GameInput(inputState);
            }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
            {
            if (this._content == null)
                {
                this._content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

            this._headsUpDisplay.LoadContent(this._content);
            this._world = LoadWorld(this._gameStartParameters.World);
            GlobalServices.SetWorld(this._world);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            this.ScreenManager.Game.Deactivated += GameOnDeactivated;
            }

        private void GameOnDeactivated(object sender, EventArgs e)
            {
            this._isGamePaused = true;
            }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
            {
            this._content.Unload();
            this.ScreenManager.Game.Deactivated -= GameOnDeactivated;
            }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);

            if (this._isGamePaused || !this.IsActive)
                gameTime = new GameTime();

            GlobalServices.SoundPlayer.ActiveSoundService.Update();

            if (!this._isGamePaused && gameTime.ElapsedGameTime != TimeSpan.Zero)
                {
                // ReSharper disable once PossibleNullReferenceException
                LevelReturnType lrt = this._world.Update(gameTime);
                switch (lrt)
                    {
                    case LevelReturnType.FinishedWorld:
                        //this._world = null;
                        //this._lives++;
                        this.ScreenManager.Game.Exit();
                        break;
            
                    case LevelReturnType.LostLife:
                        GlobalServices.SoundPlayer.ActiveSoundService.Clear();
                        if (this._lives == 0)
                            {
                            this.ScreenManager.Game.Exit();
                            return;
                            }
                        this._lives--;
                        this._world.ResetLevelAfterLosingLife();
                        break;
                    }
                }

            if (gameTime.IsRunningSlowly)
                {
                string text = $"{gameTime.TotalGameTime}: Running slowly";
                System.Diagnostics.Trace.WriteLine(text);
                }
            }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(/*InputState input*/)
            {
            var gameInput = this._gameInput;
            gameInput.Update();

            if (gameInput.HasGameExitBeenTriggered)
                {
                //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
                this.ScreenManager.Game.Exit();
                }

            if (gameInput.HasPauseBeenTriggered)
                this._isGamePaused = !this._isGamePaused;

            if (gameInput.HasToggleFullScreenBeenTriggered)
                this.ScreenManager.ToggleFullScreen();
                    
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
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (this._isGamePaused)
                gameTime = new GameTime();

            // Draw the sprite.
            if (this._world != null)
                {
                spriteBatch.Begin(this._world.WorldWindow.WindowPosition);
                this._world.Draw(gameTime, spriteBatch);
                this._headsUpDisplay.DrawStatus(spriteBatch, GlobalServices.GameState.Player.IsExtant, GlobalServices.GameState.Player.Energy, this._scoreKeeper.CurrentScore, this._lives, this._isGamePaused, gameTime.IsRunningSlowly);
                spriteBatch.End();

                base.Draw(gameTime);
                }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
            }

        public World LoadWorld(string worldData)
            {
            // Load the World.
            var world = new World(this._worldLoader, worldData);
            world.ResetLevelForStartingNewLife();
            return world;
            }

        }
    }
