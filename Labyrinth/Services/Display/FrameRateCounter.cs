using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class FrameRateCounter : DrawableGameComponent
        {
        private readonly ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game) : base(game)
            {
            this._content = new ContentManager(game.Services, game.Content.RootDirectory);
            }

        protected override void LoadContent()
            {
            this._spriteBatch = new SpriteBatch(GraphicsDevice);
            this._spriteFont = this._content.Load<SpriteFont>("Display/FpsFont");
            }

        protected override void UnloadContent()
            {
            this._content.Unload();
            }

        public override void Update(GameTime gameTime)
            {
            this._elapsedTime += gameTime.ElapsedGameTime;

            if (this._elapsedTime > TimeSpan.FromSeconds(1))
                {
                this._elapsedTime -= TimeSpan.FromSeconds(1);
                this._frameRate = this._frameCounter;
                this._frameCounter = 0;
                }
            }

        public override void Draw(GameTime gameTime)
            {
            this._frameCounter++;

            string fps = $"fps: {this._frameRate}";

            this._spriteBatch.Begin();

            this._spriteBatch.DrawString(this._spriteFont, fps, new Vector2(33, 33), Color.Black);
            this._spriteBatch.DrawString(this._spriteFont, fps, new Vector2(32, 32), Color.White);
            
            this._spriteBatch.End();
            }
        }
    }
