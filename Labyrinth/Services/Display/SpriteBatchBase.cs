using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public abstract class SpriteBatchBase : ISpriteBatch
        {
        private readonly Texture2D _pointTexture;
        private Vector2 _windowPosition;
        protected float Zoom;
        protected int ScreenCentreWidth;

        protected SpriteBatch SpriteBatch { get; }
        
        protected SpriteBatchBase(GraphicsDevice graphicsDevice)
            {
            this.SpriteBatch = new SpriteBatch(graphicsDevice);
            this._pointTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._pointTexture.SetData(new [] { Color.White });
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
        public void DrawEntireTextureInWindow(Texture2D texture, Vector2 relativePosition)
            {
            var absolutePosition = relativePosition - this._windowPosition;
            this.DrawEntireTexture(texture, absolutePosition);
            }

        /// <inheritdoc />
        public void DrawEntireTexture(Texture2D texture, Vector2 absolutePosition)
            {
            this.DrawTexture(texture, absolutePosition, null, 1f, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        public void DrawTexture(DrawParameters drawParameters)
            {
            var absolutePosition = drawParameters.Position - this._windowPosition;
            this.DrawTexture(drawParameters.Texture, absolutePosition, drawParameters.AreaWithinTexture, drawParameters.Opacity, drawParameters.Rotation, drawParameters.Centre, drawParameters.Effects);
            }

        /// <inheritdoc />
        public void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle)
            {
            this.DrawTexture(texture, absolutePosition, sourceRectangle, 1f, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        protected abstract void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float opacity, float rotation, Vector2 origin, SpriteEffects effects);

        /// <inheritdoc />
        public virtual void DrawRectangle(Rectangle destinationRectangle, Color color)
            {
            this.DrawTexture(this._pointTexture, destinationRectangle, color);
            }

        protected abstract void DrawTexture(Texture2D texture, Rectangle absoluteArea, Color colour);

        /// <inheritdoc />
        public void DrawCentredString(SpriteFont font, string text, int y, Color color)
            {
            float textWidth = font.MeasureString(text).X;
            Vector2 pos = new Vector2(this.ScreenCentreWidth, y);
            Vector2 origin = new Vector2(textWidth / 2f, 0f);
            this.SpriteBatch.DrawString(spriteFont: font, text: text, position: pos, color: color, rotation: 0, origin: origin, scale: this.Zoom, effects: SpriteEffects.None, layerDepth: 0);
            }
        }
    }
