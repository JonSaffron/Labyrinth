using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteBatchFullScreen : SpriteBatchBase, ISpriteBatch
        {
        private readonly float _zoom;
        private readonly Point _offset;
        
        public SpriteBatchFullScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice)
            {
            this._zoom = GetFullScreenZoom(graphicsDevice.Viewport);
            this._offset = GetFullScreenOffset(graphicsDevice.Viewport, this._zoom);
            }

        protected override void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects)
            {
            var destination = new Vector2(position.X * this._zoom + this._offset.X, position.Y * this._zoom + this._offset.Y);
            this.SpriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, this._zoom, effects, 0);
            }

        protected override void DrawTexture(Texture2D texture, Rectangle absolutePosition, Color colour)
            {
            var x = (int)(absolutePosition.X * this._zoom);
            var y = (int)(absolutePosition.Y * this._zoom);
            var width = (int)(absolutePosition.Width * this._zoom);
            var height = (int)(absolutePosition.Height * this._zoom);

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
        }
    }
