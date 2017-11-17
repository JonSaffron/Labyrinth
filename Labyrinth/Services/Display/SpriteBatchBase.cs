using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public abstract class SpriteBatchBase : ISpriteBatch
        {
        private readonly Texture2D _rectangleTexture;
        private Vector2 _windowPosition;
        protected float Zoom;
        protected int ScreenCentreWidth;

        protected SpriteBatch SpriteBatch { get; }
        
        protected SpriteBatchBase(GraphicsDevice graphicsDevice)
            {
            this.SpriteBatch = new SpriteBatch(graphicsDevice);
            this._rectangleTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._rectangleTexture.SetData(new [] { Color.White });
            }

        /// <inheritdoc />
        public virtual void Begin(Vector2 windowPosition)
            {
            this._windowPosition = windowPosition;
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }


        /// <inheritdoc />
        public virtual void End()
            {
            this.SpriteBatch.End();
            }

        /// <inheritdoc />
        public virtual void Dispose()
            {
            if (!this.SpriteBatch.IsDisposed)
                {
                this.SpriteBatch.Dispose();
                }
            }

        /// <inheritdoc />
        public virtual void DrawEntireTextureInWindow(Texture2D texture, Vector2 relativePosition)
            {
            var absolutePosition = relativePosition - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        /// <inheritdoc />
        public virtual void DrawEntireTexture(Texture2D texture, Vector2 position)
            {
            this.DrawTexture(texture, position, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        /// <inheritdoc />
        public virtual void DrawTextureInWindow(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var absolutePosition = position - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, sourceRectangle, rotation, origin, effects);
            }

        /// <inheritdoc />
        public virtual void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle)
            {
            this.DrawTexture(texture, absolutePosition, sourceRectangle, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        protected abstract void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects);

        /// <inheritdoc />
        public virtual void DrawRectangle(Rectangle destinationRectangle, Color color)
            {
            this.DrawTexture(this._rectangleTexture, destinationRectangle, color);
            }

        protected abstract void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour);

        /// <inheritdoc />
        public void DrawCentredString(SpriteFont font, string text, int y, Color color)
            {
            // todo can the same effect be achieved by setting the origin to the middle of the text?
            Vector2 size = font.MeasureString(text) * this.Zoom;
            Vector2 pos = new Vector2(this.ScreenCentreWidth - size.X / 2f, y * this.Zoom);
            this.SpriteBatch.DrawString(spriteFont: font, text: text, position: pos, color: color, rotation: 0, origin: Vector2.Zero, scale: this.Zoom, effects: SpriteEffects.None, layerDepth: 0);
            }
        }
    }
