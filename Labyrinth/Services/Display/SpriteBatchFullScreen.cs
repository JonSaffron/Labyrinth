using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    class SpriteBatchFullScreen : ISpriteBatch
        {
        private readonly SpriteBatch _spriteBatch;
        private readonly float _zoom;
        private readonly Vector2 _offset;
        private readonly Texture2D _rectangleTexture;

        public Vector2 WindowOffset { get; set; }
        
        public SpriteBatchFullScreen(GraphicsDevice graphicsDevice)
            {
            this._spriteBatch = new SpriteBatch(graphicsDevice);
            this._zoom = GetFullScreenZoom(graphicsDevice);
            this._offset = GetFullScreenOffset(graphicsDevice);

            this._rectangleTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._rectangleTexture.SetData(new [] { Color.White });
            }

        public void Begin()
            {
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

        public void DrawEntireTexture(Texture2D texture, Vector2 position)
            {
            this.DrawTexture(texture, position, null, 0.0f, Vector2.Zero);
            }
        
        public void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None)
            {
            Vector2 destination = (position - WindowOffset) * _zoom + this._offset;
            this._spriteBatch.Draw(texture, destination, sourceRectangle, Color.White, rotation, origin, _zoom, effects, 0);
            }
        
        public void DrawRectangle(Rectangle destinationRectangle, Color color)
            {
            var x = (int)(destinationRectangle.X * _zoom);
            var y = (int)(destinationRectangle.Y * _zoom);
            var width = (int)(destinationRectangle.Width * _zoom);
            var height = (int)(destinationRectangle.Height * _zoom);
            var r = new Rectangle(x + (int)this._offset.X, y + (int)this._offset.Y, width, height);
            this._spriteBatch.Draw(this._rectangleTexture, r, color);
            }

        private static float GetFullScreenZoom(GraphicsDevice graphicsDevice)
            {
            float zoomHeight = (float) graphicsDevice.Viewport.Height / Game1.RoomSizeHeight;
            float zoomWidth = (float) graphicsDevice.Viewport.Width / Game1.RoomSizeWidth;
            var result = Math.Min(zoomHeight, zoomWidth);
            return result;
            }

        private static Vector2 GetFullScreenOffset(GraphicsDevice graphicsDevice)
            {
            var zoom = GetFullScreenZoom(graphicsDevice);
            var viewx = Game1.RoomSizeWidth * zoom;
            var viewy = Game1.RoomSizeHeight * zoom;
            var offsetx = (graphicsDevice.Viewport.Width - viewx) / 2;
            var offsety = (graphicsDevice.Viewport.Height - viewy) / 2;
            var result = new Vector2(offsetx, offsety);
            return result;
            }
        }
    }
