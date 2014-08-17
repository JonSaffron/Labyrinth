using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public class SpriteBatchWindowed : SpriteBatch
        {
        public float Zoom { get; set; }
        public Vector2 WindowOffset { get; set; }
        
        public SpriteBatchWindowed(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
            Zoom = 1;
            }
        
        public void DrawInWindow(Texture2D texture, Vector2 position)
            {
            this.DrawInWindow(texture, position, null, 0.0f, Vector2.Zero);
            }
        
        public void DrawInWindow(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None)
            {
            Vector2 destination = (position - WindowOffset) * Zoom;
            base.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, Zoom, effects, 0);
            }
        
        public void DrawInWindow(Texture2D texture, Rectangle destinationRectangle, Color color)
            {
            var x = (int)(destinationRectangle.X * Zoom);
            var y = (int)(destinationRectangle.Y * Zoom);
            var width = (int)(destinationRectangle.Width * Zoom);
            var height = (int)(destinationRectangle.Height * Zoom);
            var r = new Rectangle(x, y, width, height);
            base.Draw(texture, r, color);
            }
        }
    }
