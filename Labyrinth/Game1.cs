using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        public const int RoomSizeWidth = 16 * Tile.Width;
        public const int RoomSizeHeight = 10 * Tile.Height;
        private const int ZoomWhilstWindowed = 2;

        public IPlayerInput PlayerInput { get; private set; }
        public ISoundPlayer SoundPlayer { get; private set; }
        public ISpriteBatch SpriteBatch { get; private set; }
        internal World World { get; private set; }
        
        private readonly GraphicsDeviceManager _gdm;
        private readonly SoundLibrary _soundLibrary = new SoundLibrary();
        private readonly IWorldLoader _worldLoader;

        private Texture2D _digits;
        private Texture2D _life;
        private int _score;
        private int _displayedScore;
        private int _lives;
        
        internal Game1(IPlayerInput playerInput, IWorldLoader worldLoader)
            {
            if (playerInput == null)
                throw new ArgumentNullException("playerInput");
            if (worldLoader == null)
                throw new ArgumentNullException("worldLoader");
            this.PlayerInput = playerInput;
            this.PlayerInput.GameInput = new GameInput(this);
            this._worldLoader = worldLoader;
            
            this._gdm = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferWidth = RoomSizeWidth * ZoomWhilstWindowed,
                                PreferredBackBufferHeight = RoomSizeHeight * ZoomWhilstWindowed
                            };
            this.Content.RootDirectory = "Content";
            //this.TargetElapsedTime = new TimeSpan(this.TargetElapsedTime.Ticks * 4);
            this._lives = 2;
            this._score = 0;
            this._displayedScore = 0;
            this.SoundPlayer = new NullSoundPlayer();
            }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
            {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.SpriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);

            this._digits = Content.Load<Texture2D>("Display/Digits");
            this._life = Content.Load<Texture2D>("Display/Life");

            this._soundLibrary.LoadContent(this.Content);
            this.SetSoundPlayer(1, 0);
            }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
            {
            if (this.World == null) 
                return;
            
            this.World.Dispose();
            this.SpriteBatch.Dispose();
            this.World = null;
            }

        public void AddScore(int score)
            {
            if (score <= 0)
                throw new ArgumentOutOfRangeException("score");

            this._score += score;
            }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update</param>
        protected override void Update(GameTime gameTime)
            {
            if (this.World == null)
                {
                LoadLevel("World1.xml");
                }
            

            base.Update(gameTime);
            this._soundLibrary.CheckForStoppedInstances();

            // ReSharper disable once PossibleNullReferenceException
            LevelReturnType lrt = this.World.Update(gameTime);
            switch (lrt)
                {
                case LevelReturnType.FinishedLevel:
                    this.World.Dispose();
                    this.World = null;
                    this._lives++;
                    break;
                
                case LevelReturnType.LostLife:
                    if (this._lives == 0)
                        {
                        this.Exit();
                        return;
                        }
                    this._lives--;
                    this.World.ResetLevelAfterLosingLife(SpriteBatch);
                    break;
                }

            if (gameTime.IsRunningSlowly)
                {
                string text = string.Format("{0}: Running slowly", gameTime.TotalGameTime);
                System.Diagnostics.Trace.WriteLine(text);
                }

            if (this._displayedScore < this._score)
                this._displayedScore++;

            ProcessGameInput();
            }

        private void ProcessGameInput()
            {
            var gameInput = this.PlayerInput.GameInput;
            if (gameInput.HasGameExitBeenTriggered)
                this.Exit();

            if (gameInput.HasToggleFullScreenBeenTriggered)
                ToggleFullScreen();
                
            int changeToEnabled = (gameInput.HasSoundOnBeenTriggered ? 1 : 0) + (gameInput.HasSoundOffBeenTriggered ? -1 : 0);
            int changeToVolume = (gameInput.HasSoundIncreaseBeenTriggered ? 1 : 0) + (gameInput.HasSoundDecreaseBeenTriggered  ? -1 : 0);
            SetSoundPlayer(changeToEnabled, changeToVolume);

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
            SpriteBatch.Begin();
            if (this.World != null)
                {
                this.World.Draw(gameTime, SpriteBatch);
                DrawStatus(SpriteBatch);
                }
            SpriteBatch.End();

            base.Draw(gameTime);
            }
            
        internal void LoadLevel(string level)
            {
            // Load the World.
            this._worldLoader.LoadWorld(level);
            this.World = new World(this, this._worldLoader);
            this.World.ResetLevelForStartingNewLife(this.SpriteBatch);
            }
        
        private void DrawStatus(ISpriteBatch spriteBatch)
            {
            DrawEnergy(spriteBatch);
            DrawScoreAndLives(spriteBatch);
            }
        
        private void DrawEnergy(ISpriteBatch spriteBatch)
            {
            var r = new Rectangle(22, 6, 148, 20);
            spriteBatch.DrawRectangle(r, Color.Blue);
            
            r.Inflate(-2, -2);
            spriteBatch.DrawRectangle(r, Color.Black);
            
            r = new Rectangle(32, 12, 128, 8);
            spriteBatch.DrawRectangle(r, Color.Blue);
            
            if (!this.World.Player.IsExtant)
                return;
            
            bool isAboutToDie = this.World.Player.Energy < 4;
            int barLength = isAboutToDie ? (this.World.Player.Energy + 1) << 4 : Math.Min(this.World.Player.Energy >> 2, 64);
            Color barColour = isAboutToDie ? Color.Red : Color.Green;
            r = new Rectangle(32, 12, barLength * 2, 8);
            spriteBatch.DrawRectangle(r, barColour);

#if DEBUG
            r = new Rectangle(168, 8, 28, 16);
            spriteBatch.DrawRectangle(r, Color.Black);
            DrawValue(spriteBatch, this.World.Player.Energy, 168 + 24, 8);
#endif
            }

        private void DrawScoreAndLives(ISpriteBatch spriteBatch)
            {
            var r = new Rectangle(342, 6, 148, 20);
            spriteBatch.DrawRectangle(r, Color.Blue);
            
            r.Inflate(-2, -2);
            spriteBatch.DrawRectangle(r, Color.Black);
            
            DrawValue(spriteBatch, this._displayedScore, 416, 8);

            for (int i = 0; i < this._lives; i++)
                {
                Vector2 destination = new Vector2(480 - ((i + 1) * 16), 8) + spriteBatch.WindowOffset;
                spriteBatch.DrawEntireTexture(this._life, destination);
                }
            }

        private void DrawValue(ISpriteBatch spriteBatch, int value, int right, int top)
            {
            int i = 1;
            while (true)
                {
                int digit = value % 10;
                var source = new Rectangle(digit * 6, 0, 6, 16);
                Vector2 destination = new Vector2(right - (i * 8), top) + spriteBatch.WindowOffset;
                spriteBatch.DrawTexture(this._digits, destination, source, 0.0f, Vector2.Zero);
                value = value / 10;
                if (value == 0)
                    break;
                i++;
                }
            }

        private void ToggleFullScreen()
            {
            this._gdm.ToggleFullScreen();
            var windowOffset = this.SpriteBatch.WindowOffset;
            this.SpriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);
            SpriteBatch.WindowOffset = windowOffset;
            }

        private static ISpriteBatch GetSpriteBatch(GraphicsDevice graphicsDevice, bool isFullScreen)
            {
            var result = isFullScreen ? (ISpriteBatch) new SpriteBatchFullScreen(graphicsDevice) : new SpriteBatchWindowed(graphicsDevice, ZoomWhilstWindowed);
            return result;
            }

        /// <summary>
        /// Updates the sound player properties
        /// </summary>
        /// <param name="changeToEnabled">If set to a positive value then sound will be enabled, if set to a negative value then sound will be disabled</param>
        /// <param name="changeToVolume">If set to a positive value then the volume will be increased, if set to a negative value then the volume will be decreased.</param>
        private void SetSoundPlayer(int changeToEnabled, int changeToVolume)
            {
            if (changeToVolume != 0)
                {
                float newVolume = SoundEffect.MasterVolume + Math.Sign(changeToVolume) * 0.1f;
                newVolume = Math.Min(1.0f, Math.Max(0.0f, newVolume));
                SoundEffect.MasterVolume = newVolume;
                }

            if (changeToEnabled > 0 && !(this.SoundPlayer is SoundPlayer))
                this.SoundPlayer = new SoundPlayer(this._soundLibrary);
            else if (changeToEnabled < 0 && !(this.SoundPlayer is NoSoundPlayer))
                this.SoundPlayer =  new NoSoundPlayer(this._soundLibrary);
            }
        }
    }
