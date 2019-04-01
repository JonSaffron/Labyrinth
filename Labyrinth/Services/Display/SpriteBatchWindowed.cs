using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchWindowed : SpriteBatchBase
        {
        public SpriteBatchWindowed(GraphicsDevice graphicsDevice, float zoom) : base(graphicsDevice)
            {
            this.Zoom = zoom;
            this.ScreenCentreWidth = graphicsDevice.Viewport.Width / 2;
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float opacity, float rotation, Vector2 origin, SpriteEffects effects)
            {
            Vector2 destination = position * this.Zoom;
            var colour = new Color(1f, 1f, 1f, opacity);
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, colour, rotation, origin, this.Zoom, effects, 0);
            }
        
        protected override void DrawTexture(Texture2D texture, Rectangle absoluteArea, Color colour)
            {
            var x = (int)(absoluteArea.X * this.Zoom);
            var y = (int)(absoluteArea.Y * this.Zoom);
            var width = (int)(absoluteArea.Width * this.Zoom);
            var height = (int)(absoluteArea.Height * this.Zoom);

            var r = new Rectangle(x, y, width, height);
            this.SpriteBatch.Draw(texture, r, colour);
            }
        }
    }
