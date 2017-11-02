using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchWindowed : SpriteBatchBase, ISpriteBatch
        {
        public SpriteBatchWindowed(GraphicsDevice graphicsDevice, float zoom) : base(graphicsDevice)
            {
            this.Zoom = zoom;
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            Vector2 destination = position * this.Zoom;
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, this.Zoom, effects, 0);
            }
        
        protected override void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour)
            {
            var x = (int)(absolutePosition.X * this.Zoom);
            var y = (int)(absolutePosition.Y * this.Zoom);
            var width = (int)(absolutePosition.Width * this.Zoom);
            var height = (int)(absolutePosition.Height * this.Zoom);

            var r = new Rectangle(x, y, width, height);
            this.SpriteBatch.Draw(texture, r, colour);
            }

        public void DrawString(SpriteFont font, string text, Vector2 pos, Color color, Vector2 origin)
            {
            this.SpriteBatch.DrawString(font, text, pos, color, 0, origin, this.Zoom, SpriteEffects.None, 0);
            }

        public float Zoom { get; }
        }
    }
