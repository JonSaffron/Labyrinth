using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchWindowed : SpriteBatchBase, ISpriteBatch
        {
        private readonly float _zoom;

        public SpriteBatchWindowed(GraphicsDevice graphicsDevice, float zoom) : base(graphicsDevice)
            {
            this._zoom = zoom;
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            Vector2 destination = position * this._zoom;
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, this._zoom, effects, 0);
            }
        
        protected override void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour)
            {
            var x = (int)(absolutePosition.X * this._zoom);
            var y = (int)(absolutePosition.Y * this._zoom);
            var width = (int)(absolutePosition.Width * this._zoom);
            var height = (int)(absolutePosition.Height * this._zoom);

            var r = new Rectangle(x, y, width, height);
            this.SpriteBatch.Draw(texture, r, colour);
            }
        }
    }
