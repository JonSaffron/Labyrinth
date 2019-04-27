using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    class ScreenWipe : GameScreen
        {
        private GraphicsDevice _graphicsDevice;
        private Texture2D _blankTexture;
        private SpriteBatch _spriteBatch;
        private Texture2D _transitionTexture;

        public delegate void WipedEventHandler(object source, EventArgs eventArgs);
        public event WipedEventHandler Wiped;

        public ScreenWipe()
            {
            this.IsPopup = true;
            this.TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            }

        public override void LoadContent()
            {
            this._graphicsDevice = this.ScreenManager.GraphicsDevice;
            this._blankTexture = new Texture2D(this._graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._blankTexture.SetData(new[] { Color.White });
            this._spriteBatch = new SpriteBatch(this._graphicsDevice);
            }

        public override void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);
            if (this.ScreenState == ScreenState.Active)
                {
                Wiped?.Invoke(this, new EventArgs());
                }
            else
                {
                this._transitionTexture = BuildTransitionTexture(1f - this.TransitionAlpha);
                }
            }

        public override void Draw(GameTime gameTime)
            {
            if (this._transitionTexture == null)
                return;

            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawTexture(this._transitionTexture, Vector2.Zero, null);
            spriteBatch.End();
            }

        private Texture2D BuildTransitionTexture(float alpha)
            {
            int cx = (int) Constants.RoomSizeInPixels.X;
            int cy = (int) Constants.RoomSizeInPixels.Y;
            RenderTarget2D target = new RenderTarget2D(this._graphicsDevice, cx, cy, false, SurfaceFormat.Color, DepthFormat.None);
            
            this._graphicsDevice.SetRenderTarget(target);
            this._graphicsDevice.Clear(Color.Black);

            this._spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);

            int x = (int)(cx * alpha / 2.0f);
            int y = (int)(cy * alpha / 2.0f);
            if (x > 0 && y > 0)
                {
                var whiteRectangle = new Rectangle(cx / 2 - x, cy / 2 - y, 2 * x, 2 * y);
                this._spriteBatch.Draw(this._blankTexture, whiteRectangle, Color.White);

                x -= 1;
                y -= 1;
                if (x > 0 && y > 0)
                    {
                    var transparentRectangle = new Rectangle(cx / 2 - x, cy / 2 - y, 2 * x, 2 * y);
                    this._spriteBatch.Draw(this._blankTexture, transparentRectangle, Color.Transparent);
                    }
                }

            this._spriteBatch.End();
            this._graphicsDevice.SetRenderTarget(null);

            return target;
            }
        }
    }
