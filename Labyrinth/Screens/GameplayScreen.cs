using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Labyrinth.Services.Messages;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// This screen implements the actual game. 
    /// </summary>
    internal class GameplayScreen : GameScreen
        {
        private readonly WorldStartParameters _worldStartParameters;
        private readonly ContentManager _content;
        private readonly IHeadsUpDisplay _headsUpDisplay = new HeadsUpDisplay();
        private readonly GameInput _gameInput;
        private World? _world;
        private int _livesRemaining;
        private bool _normalPlay = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(WorldStartParameters worldStartParameters, InputState inputState)
            {
            _worldStartParameters = worldStartParameters ?? throw new ArgumentNullException(nameof(worldStartParameters));
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            _gameInput = new GameInput(inputState);
            _livesRemaining = worldStartParameters.CountOfLives - 1;
            GlobalServices.ScoreKeeper.Reset();
            _content = new ContentManager(GlobalServices.Game.Services, GlobalServices.Game.Content.RootDirectory);
            VolumeControl.Instance.LoadState();
            }

        /// <summary>
        /// Loads the world - this could take some time
        /// </summary>
        public override void Activate(bool instancePreserved = false)
            {
            _headsUpDisplay.LoadContent(_content);

            var worldLoader = new WorldLoader(_worldStartParameters.WorldToLoad);
            var boundMovementFactory = new BoundMovementFactory(worldLoader.WorldSize);
            GlobalServices.SetBoundMovementFactory(boundMovementFactory);

            GlobalServices.ClearWorld();
            _world = new World(worldLoader);
            //GlobalServices.GameState.AddPotion(GlobalServices.GameState.Player.TilePosition.GetPositionAfterMoving(Direction.Right, 4).ToPosition());
            GlobalServices.SetWorld(_world);

            _world.LoadContent(_content);
            _world.ResetWorldForStartingNewLife();

            var msg = new WorldStatus(worldLoader.WorldName);
            Messenger.Default.Send(msg);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            ScreenManager.Game.Deactivated += GameOnDeactivated;
            }

        private void GameOnDeactivated(object? sender, EventArgs e)
            {
            if (this.ScreenState == ScreenState.Active && this.ScreenManager.Screens.Count() == 1)
                {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                }
            }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
            {
            _content.Unload();
            ScreenManager.Game.Deactivated -= GameOnDeactivated;
            }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);

            if (_world == null)
                {
                return;
                }

            if (!IsActive)
                gameTime = new GameTime();

            GlobalServices.SoundPlayer.ActiveSoundService.Update();

            if (gameTime.ElapsedGameTime != TimeSpan.Zero)
                {
                // ReSharper disable once PossibleNullReferenceException
                WorldState worldState = _world.Update(gameTime);
                if (worldState == WorldState.Normal)
                    {
                    _normalPlay = true;
                    }
                else if (_normalPlay)
                    {
                    _normalPlay = false;
                    switch (worldState)
                        {
                        case WorldState.FinishedWorld:
                            //this._world = null;
                            //this._livesRemaining++;
                            // todo this isn't the ideal place for this
                            VolumeControl.Instance.SaveState();
                            LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(), new GameOverScreen("World Completed"));
                            break;

                        case WorldState.LostLife:
                            var screenWipe = new ScreenWipe();
                            screenWipe.Wiped += LostLife;
                            ScreenManager.AddScreen(screenWipe, ControllingPlayer);
                            break;
                        }
                    }
                }

            if (gameTime.IsRunningSlowly)
                {
                string text = $"{gameTime.TotalGameTime}: Running slowly";
                System.Diagnostics.Trace.WriteLine(text);
                }
            }

        private void LostLife(object source, EventArgs eventArgs)
            {
            GlobalServices.SoundPlayer.ActiveSoundService.Clear();
            ScreenManager.RemoveScreen((GameScreen)source);
            if (_livesRemaining == 0)
                {
                // todo this isn't the ideal place for this
                VolumeControl.Instance.SaveState();
                LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(), new GameOverScreen("Game Over"));
                return;
                }
            _livesRemaining--;
            _world!.ResetWorldAfterLosingLife();
            ScreenState = ScreenState.TransitionOn;
            TransitionPosition = 1f;
            }

        /// <summary>
        /// Responds to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(/*InputState input*/)
            {
            var gameInput = _gameInput;
            gameInput.Update();

            if (gameInput.HasGameExitBeenTriggered)
                {
                //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
                ScreenManager.Game.Exit();
                }

            if (gameInput.HasPauseBeenTriggered)
                {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                }

            if (gameInput.HasToggleFullScreenBeenTriggered)
                ScreenManager.ToggleFullScreen();

            if (gameInput.HasIncreaseZoomBeenTriggered)
                ScreenManager.IncreaseZoom();

            if (gameInput.HasDecreaseZoomBeenTriggered)
                ScreenManager.DecreaseZoom();

            int changeToEnabled = (gameInput.HasSoundOnBeenTriggered ? 1 : 0) + (gameInput.HasSoundOffBeenTriggered ? -1 : 0);
            if (changeToEnabled < 0)
                VolumeControl.Instance.Mute();
            else if (changeToEnabled > 0)
                VolumeControl.Instance.Unmute();

            int changeToVolume = (gameInput.HasSoundIncreaseBeenTriggered ? 1 : 0) + (gameInput.HasSoundDecreaseBeenTriggered ? -1 : 0);
            if (changeToVolume < 0)
                VolumeControl.Instance.TurnDownTheVolume();
            else if (changeToVolume > 0)
                VolumeControl.Instance.TurnUpTheVolume();

            if (gameInput.HasMoveToNextLevelBeenTriggered)
                {
                _world?.MoveUpALevel();
                }
            }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (!this.IsActive)
                gameTime = new GameTime();

            // Draw the screen
            _world?.Draw(spriteBatch);
            _headsUpDisplay.DrawStatus(spriteBatch, gameTime, GlobalServices.GameState.Player.IsExtant, GlobalServices.GameState.Player.Energy, GlobalServices.ScoreKeeper.CurrentScore, this._livesRemaining);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0f)
                {
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
                }
            }
        }
    }
