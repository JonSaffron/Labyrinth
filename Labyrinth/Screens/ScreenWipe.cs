using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Screens
    {
    internal class ScreenWipe : GameScreen
        {
        private SpriteBatch? _spriteBatch;
        private Texture2D? _transitionTexture;

        public delegate void WipedEventHandler(object source, EventArgs eventArgs);
        public event WipedEventHandler? Wiped;

        public ScreenWipe()
            {
            this.IsPopup = true;
            this.TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            }

        public override void Activate(bool instancePreserved = false)
            {
            if (this.ScreenManager == null)
                throw new InvalidOperationException("ScreenManager is not set");
            this._spriteBatch = new SpriteBatch(this.ScreenManager.GraphicsDevice);
            }

        private SpriteBatch SpriteBatch
            {
            get
                {
                if (this._spriteBatch == null)
                    throw new InvalidOperationException("SpriteBatch has not been instantiated");
                return this._spriteBatch;
                }
            }

        public override void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);
            if (this.ScreenState == ScreenState.Active)
                {
                Wiped?.Invoke(this, EventArgs.Empty);
                }
            else
                {
                this._transitionTexture = BuildTransitionTexture(this.ScreenManager, 1f - this.TransitionAlpha);
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

        private Texture2D BuildTransitionTexture(ScreenManager screenManager, float alpha)
            {
            var graphicsDevice = screenManager.GraphicsDevice;
            int cx = (int) Constants.RoomSizeInPixels.X;
            int cy = (int) Constants.RoomSizeInPixels.Y;
            RenderTarget2D target = new RenderTarget2D(graphicsDevice, cx, cy, false, SurfaceFormat.Color, DepthFormat.None);
            
            graphicsDevice.SetRenderTarget(target);
            graphicsDevice.Clear(Color.Black);

            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);

            int x = (int)(cx * alpha / 2.0f);
            int y = (int)(cy * alpha / 2.0f);
            if (x > 0 && y > 0)
                {
                var whiteRectangle = new Rectangle(cx / 2 - x, cy / 2 - y, 2 * x, 2 * y);
                this.SpriteBatch.Draw(screenManager.BlankTexture, whiteRectangle, Color.White);

                x -= 1;
                y -= 1;
                if (x > 0 && y > 0)
                    {
                    var transparentRectangle = new Rectangle(cx / 2 - x, cy / 2 - y, 2 * x, 2 * y);
                    this.SpriteBatch.Draw(screenManager.BlankTexture, transparentRectangle, Color.Transparent);
                    }
                }

            this.SpriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            return target;
            }
        }
    }
