using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public abstract class SpriteBatchBase : IDisposable
        {
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _rectangleTexture;
        private Vector2 _windowPosition;

        protected SpriteBatchBase(GraphicsDevice graphicsDevice)
            {
            this._spriteBatch = new SpriteBatch(graphicsDevice);
            this._rectangleTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._rectangleTexture.SetData(new [] { Color.White });
            }

        public virtual void Begin(Vector2 windowPosition)
            {
            this._windowPosition = windowPosition;
            this._spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

        public virtual void End()
            {
            this._spriteBatch.End();
            }

        public virtual void Dispose()
            {
            if (!this._spriteBatch.IsDisposed)
                {
                this._spriteBatch.Dispose();
                }
            }

        public virtual void DrawEntireTextureInWindow(Texture2D texture, Vector2 relativePosition)
            {
            var absolutePosition = relativePosition - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        public virtual void DrawEntireTexture(Texture2D texture, Vector2 position)
            {
            this.DrawTexture(texture, position, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        public virtual void DrawTextureInWindow(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var absolutePosition = position - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, sourceRectangle, rotation, origin, effects);
            }

        public virtual void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle)
            {
            this.DrawTexture(texture, absolutePosition, sourceRectangle, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        protected abstract void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects);

        public virtual void DrawRectangle(Rectangle destinationRectangle, Color color)
            {
            this.DrawTexture(this._rectangleTexture, destinationRectangle, color);
            }

        protected abstract void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour);

        public abstract void DrawString(SpriteFont font, string text, Vector2 pos, Color color, Vector2 origin);

        protected SpriteBatch SpriteBatch => this._spriteBatch;
        }
    }
