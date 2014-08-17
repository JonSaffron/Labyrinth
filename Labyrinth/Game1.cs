using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth
    {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
        {
        SpriteBatchWindowed _spriteBatch;
        private World _world;

        public SoundLibrary SoundLibrary {get; private set;}

        private const int BackBufferWidth = 1024;
        private const int BackBufferHeight = 640;

        private int _score;
        private int _lives;
        
        private Texture2D _digits;
        private Texture2D _life;
        private Texture2D _rectangleTexture;
        
        private readonly GraphicsDeviceManager _gdm;

        public Game1()
            {
            this._gdm = new GraphicsDeviceManager(this)
                            {
                                PreferredBackBufferWidth = BackBufferWidth,
                                PreferredBackBufferHeight = BackBufferHeight
                            };
            Content.RootDirectory = "Content";
            //this.TargetElapsedTime = new TimeSpan(this.TargetElapsedTime.Ticks * 4);
            _lives = 2;
            _score = 0;
            }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
            {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatchWindowed(GraphicsDevice) {Zoom = 2};

            this._digits = Content.Load<Texture2D>("Display/Digits");
            this._life = Content.Load<Texture2D>("Display/Life");

            _rectangleTexture = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _rectangleTexture.SetData(new [] { Color.White });

            this.SoundLibrary = new SoundLibrary(this.Content);
            }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
            {
            if (this._world == null) 
                return;
            
            this._world.Dispose();
            this._world = null;
            }

        public void AddScore(int score)
            {
            if (score <= 0)
                throw new ArgumentOutOfRangeException("score");

            this._score += score;
            }
        
        public SpriteBatchWindowed SpriteBatchWindowed
            {
            get
                {
                return this._spriteBatch;
                }
            }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
            {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                this.Exit();
                return;
                }

            SoundLibrary.CheckForStoppedInstances();

            bool newLevel = (this._world == null);
            if (newLevel)
                {
                _world = LoadLevel("World1.xml");
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                
                this.ResetElapsedTime();
                }
            
            if (!this.IsActive && !newLevel)
                return;
            
            LevelReturnType lrt = _world.Update(gameTime);
            switch (lrt)
                {
                case LevelReturnType.FinishedLevel:
                    _world.Dispose();
                    _world = null;
                    break;
                
                case LevelReturnType.LostLife:
                    if (_lives == 0)
                        this.Exit();
                    _lives--;
                    _world.ResetLevel(_spriteBatch);
                    break;
                }

            if (gameTime.IsRunningSlowly && !newLevel)
                {
                string text = string.Format("{0}: Running slowly", gameTime.TotalGameTime);
                System.Diagnostics.Trace.WriteLine(text);
                }

            base.Update(gameTime);
            }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
            {
            GraphicsDevice.Clear(Color.Black);

            // Draw the sprite.
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            if (this._world != null)
                {
                this._world.Draw(gameTime, _spriteBatch);
                DrawStatus(_spriteBatch);
                }
            _spriteBatch.End();

            base.Draw(gameTime);
            }
            
        private World LoadLevel(string levelPath)
            {
            levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Levels/" + levelPath);
            if (!File.Exists(levelPath))
                throw new Exception("No levels found.");

            // Load the World.
            var x = new WorldLoader(levelPath, this.Content);
            var result = new World(Services, x, this._spriteBatch, this);
            return result;
            }
        
        private void DrawStatus(SpriteBatchWindowed spriteBatch)
            {
            DrawEnergy(spriteBatch);
            DrawScoreAndLives(spriteBatch);
            }
        
        private void DrawEnergy(SpriteBatchWindowed spriteBatch)
            {
            float zoom = spriteBatch.Zoom;
            var r = new Rectangle(22, 6, 148, 20);
            DrawRectangle(spriteBatch, r, Color.Blue);
            
            r.Inflate(-2, -2);
            DrawRectangle(spriteBatch, r, Color.Black);
            
            if (!this._world.Player.IsExtant)
                return;
            
            int barLength;
            Color barColour;
            
            if (this._world.Player.Energy >= 4)
                {
                barLength = this._world.Player.Energy >> 2;
                if (barLength > 64)
                    barLength = 64;
                if (barLength < 64)
                    {
                    var x = (int)((16 + barLength) * zoom);
                    var w = (int)((64 - barLength) * zoom);
                    r = new Rectangle(x, 12, w, 8);
                    DrawRectangle(spriteBatch, r, Color.Blue);
                    }
                barColour = Color.Green;
                }
            else
                {
                barLength = (this._world.Player.Energy + 1) << 4;
                barColour = Color.Red;
                }
            r = new Rectangle(32, 12, (int)(barLength * spriteBatch.Zoom), 8);
            DrawRectangle(spriteBatch, r, barColour);
            }

        private void DrawScoreAndLives(SpriteBatchWindowed spriteBatch)
            {
            var r = new Rectangle(342, 6, 148, 20);
            DrawRectangle(spriteBatch, r, Color.Blue);
            
            r.Inflate(-2, -2);
            DrawRectangle(spriteBatch, r, Color.Black);
            
            int s = this._score;
            int i = 1;
            while (true)
                {
                int d = s % 10;
                var source = new Rectangle(d * 6, 0, 6, 16);
                Vector2 destination = new Vector2(416 - (i * 8), 8) + spriteBatch.WindowOffset;
                spriteBatch.DrawInWindow(this._digits, destination, source, 0.0f, Vector2.Zero);
                s = s / 10;
                if (s == 0)
                    break;
                i++;
                }
            
            //s = this._world.Player.Energy;
            //i = 1;
            //while (true)
            //    {
            //    int d = s % 10;
            //    var source = new Rectangle(d * 6, 0, 6, 16);
            //    Vector2 destination = new Vector2(500 - (i * 8), 8) + spriteBatch.WindowOffset;
            //    spriteBatch.DrawInWindow(this._digits, destination, source, Vector2.Zero, SpriteEffects.None);
            //    s = s / 10;
            //    if (s == 0)
            //        break;
            //    i++;
            //    }
            
            for (i = 0; i < this._lives; i++)
                {
                Vector2 destination = new Vector2(480 - ((i + 1) * 16), 8) + spriteBatch.WindowOffset;
                spriteBatch.DrawInWindow(this._life, destination);
                }
            
            }

        private void DrawRectangle(SpriteBatchWindowed spriteBatch, Rectangle r, Color colour)
            {
            spriteBatch.DrawInWindow(_rectangleTexture, r, colour);
            }
        }
    }
