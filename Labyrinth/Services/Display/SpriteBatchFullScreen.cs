using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchFullScreen : SpriteBatchBase, ISpriteBatch
        {
        private readonly Point _offset;
        public float Zoom { get; }
        
        public SpriteBatchFullScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
            this.Zoom = GetFullScreenZoom(graphicsDevice.Viewport);
            this._offset = GetFullScreenOffset(graphicsDevice.Viewport, this.Zoom);
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var destination = new Vector2(position.X * this.Zoom + this._offset.X, position.Y * this.Zoom + this._offset.Y);
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, this.Zoom, effects, 0);
            }

        protected override void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour)
            {
            var x = (int)(absolutePosition.X * this.Zoom);
            var y = (int)(absolutePosition.Y * this.Zoom);
            var width = (int)(absolutePosition.Width * this.Zoom);
            var height = (int)(absolutePosition.Height * this.Zoom);

            var r = new Rectangle(x, y, width, height);
            r.Offset(this._offset);
            this.SpriteBatch.Draw(texture, r, colour);
            }

        private static float GetFullScreenZoom(Viewport viewport)
            {
            float zoomHeight = viewport.Height / Constants.RoomSizeInPixels.Y;
            float zoomWidth = viewport.Width / Constants.RoomSizeInPixels.X;
            var result = Math.Min(zoomHeight, zoomWidth);
            return result;
            }

        private static Point GetFullScreenOffset(Viewport viewport, float zoom)
            {
            var viewx = Constants.RoomSizeInPixels.X * zoom;
            var viewy = Constants.RoomSizeInPixels.Y * zoom;
            var offsetx = (viewport.Width - viewx) / 2;
            var offsety = (viewport.Height - viewy) / 2;
            var result = new Point((int) offsetx, (int) offsety);
            return result;
            }

        public void DrawString(SpriteFont font, string text, Vector2 pos, Color color, Vector2 origin)
            {
            this.SpriteBatch.DrawString(font, text, pos, color, 0, origin, this.Zoom, SpriteEffects.None, 0);
            }
        }
    }
