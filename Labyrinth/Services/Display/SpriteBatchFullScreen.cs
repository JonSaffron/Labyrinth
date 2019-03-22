using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchFullScreen : SpriteBatchBase
        {
        private readonly Vector2 _offset;
        
        public SpriteBatchFullScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
            this.Zoom = GetFullScreenZoom(graphicsDevice.Viewport);
            this._offset = GetFullScreenOffset(graphicsDevice.Viewport, this.Zoom);
            this.ScreenCentreWidth = graphicsDevice.Viewport.Width / 2;
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var destination = position * this.Zoom + this._offset;
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, this.Zoom, effects, 0);
            }

        protected override void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour)
            {
            var x = (int)(absolutePosition.X * this.Zoom) + (int)this._offset.X;
            var y = (int)(absolutePosition.Y * this.Zoom) + (int)this._offset.Y;
            var width = (int)(absolutePosition.Width * this.Zoom);
            var height = (int)(absolutePosition.Height * this.Zoom);

            var r = new Rectangle(x, y, width, height);
            this.SpriteBatch.Draw(texture, r, colour);
            }

        private static float GetFullScreenZoom(Viewport viewport)
            {
            float zoomHeight = viewport.Height / Constants.RoomSizeInPixels.Y;
            float zoomWidth = viewport.Width / Constants.RoomSizeInPixels.X;
            var result = Math.Min(zoomHeight, zoomWidth);
            return result;
            }

        private static Vector2 GetFullScreenOffset(Viewport viewport, float zoom)
            {
            var viewX = Constants.RoomSizeInPixels.X * zoom;
            var viewY = Constants.RoomSizeInPixels.Y * zoom;
            var offsetX = (viewport.Width - viewX) / 2;
            var offsetY = (viewport.Height - viewY) / 2;
            var result = new Vector2((int) offsetX, (int) offsetY);
            return result;
            }
        }
    }
