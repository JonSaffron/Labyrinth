using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchWindowed : ISpriteBatch
        {
        private readonly SpriteBatch _spriteBatch;
        private readonly float _zoom;
        private readonly Texture2D _rectangleTexture;

        private Vector2 _windowPosition;

        public SpriteBatchWindowed(GraphicsDevice graphicsDevice, float zoom)
            {
            this._spriteBatch = new SpriteBatch(graphicsDevice);
            this._zoom = zoom;

            this._rectangleTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._rectangleTexture.SetData(new [] { Color.White });
            }
        
        public void Begin(Vector2 windowPosition)
            {
            this._windowPosition = windowPosition;
            this._spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

        public void End()
            {
            this._spriteBatch.End();
            }

        public void Dispose()
            {
            this.Dispose(true);
            }

        private void Dispose(bool disposing)
            {
            if (disposing && !this._spriteBatch.IsDisposed)
                {
                this._spriteBatch.Dispose();
                }
            }

        public void DrawEntireTextureInWindow(Texture2D texture, Vector2 relativePosition)
            {
            var absolutePosition = relativePosition - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        public void DrawEntireTexture(Texture2D texture, Vector2 position)
            {
            this.DrawTexture(texture, position, null, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        public void DrawTextureInWindow(Texture2D texture, Vector2 relativePosition, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var absolutePosition = relativePosition - this._windowPosition;
            this.DrawTexture(texture, absolutePosition, sourceRectangle, rotation, origin, effects);
            }

        public void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle)
            {
            this.DrawTexture(texture, absolutePosition, sourceRectangle, 0.0f, Vector2.Zero, SpriteEffects.None);
            }

        private void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            Vector2 destination = position * _zoom;
            this._spriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, _zoom, effects, 0);
            }
        
        public void DrawRectangle(Rectangle destinationRectangle, Color color)
            {
            var x = (int)(destinationRectangle.X * _zoom);
            var y = (int)(destinationRectangle.Y * _zoom);
            var width = (int)(destinationRectangle.Width * _zoom);
            var height = (int)(destinationRectangle.Height * _zoom);
            var r = new Rectangle(x, y, width, height);
            this._spriteBatch.Draw(this._rectangleTexture, r, color);
            }
        }
    }
