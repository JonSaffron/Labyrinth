using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public abstract class SpriteBatchBase : ISpriteBatch
        {
        private readonly Texture2D _pointTexture;
        public float Zoom { get; protected set; }
        public int ScreenCentreWidth { get; protected set; }

        protected SpriteBatch SpriteBatch { get; }
        
        /// <summary>
        /// Gets or sets the offset for the view on the world for relative positioning
        /// </summary>
        public Vector2 WindowPosition { get; set; }

        protected SpriteBatchBase(GraphicsDevice graphicsDevice)
            {
            this.SpriteBatch = new SpriteBatch(graphicsDevice);
            this._pointTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._pointTexture.SetData(new [] { Color.White });
            }

        /// <inheritdoc />
        public void Begin(Effect? effect = null)
            {
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, effect);
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
                GC.SuppressFinalize(this);
                }
            }

        /// <inheritdoc />
        public void DrawTexture(DrawParameters drawParameters)
            {
            var absolutePosition = drawParameters.Position - this.WindowPosition;
            this.DrawTexture(drawParameters.Texture, absolutePosition, drawParameters.AreaWithinTexture, drawParameters.Opacity, drawParameters.Rotation, drawParameters.Centre, drawParameters.Effects);
            }

        /// <inheritdoc />
        public void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle)
            {
            this.DrawTexture(texture, absolutePosition, sourceRectangle, 1f, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        protected abstract void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float opacity, float rotation, Vector2 origin, SpriteEffects effects);

        /// <inheritdoc />
        public virtual void DrawRectangle(Rectangle destinationRectangle, Color colour)
            {
            this.DrawTextureOverRegionInGameCoordinates(this._pointTexture, destinationRectangle, colour);
            }

        protected abstract void DrawTextureOverRegionInGameCoordinates(Texture2D texture, Rectangle absoluteArea, Color colour);

        public void DrawTextureOverRegion(Texture2D texture, Rectangle absoluteArea, Color colour)
            {
            this.SpriteBatch.Draw(texture, absoluteArea, colour);
            }

        /// <inheritdoc />
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color colour, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            {
            this.SpriteBatch.DrawString(spriteFont, text, position, colour, rotation, origin, scale * this.Zoom, effects, layerDepth);
            }

        /// <inheritdoc />
        public void DrawCentredString(SpriteFont font, string text, float y, Color colour)
            {
            float textWidth = font.MeasureString(text).X;
            Vector2 pos = new Vector2(this.ScreenCentreWidth, y);
            Vector2 origin = new Vector2(textWidth / 2f, 0f);
            this.SpriteBatch.DrawString(spriteFont: font, text: text, position: pos, color: colour, rotation: 0, origin: origin, scale: this.Zoom, effects: SpriteEffects.None, layerDepth: 0);
            }
        }
    }
