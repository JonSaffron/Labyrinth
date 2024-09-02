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

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float opacity, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var destination = position * this.Zoom + this._offset;
            var colour = Color.White * opacity;
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, colour, rotation, origin, this.Zoom, effects, 0);
            }

        protected override void DrawTextureOverRegionInGameCoordinates(Texture2D texture, Rectangle absoluteArea, Color colour)
            {
            var x = (int)(absoluteArea.X * this.Zoom) + (int)this._offset.X;
            var y = (int)(absoluteArea.Y * this.Zoom) + (int)this._offset.Y;
            var width = (int)(absoluteArea.Width * this.Zoom);
            var height = (int)(absoluteArea.Height * this.Zoom);

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
