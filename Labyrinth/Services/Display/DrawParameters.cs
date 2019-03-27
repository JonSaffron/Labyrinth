using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public struct DrawParameters
        {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle AreaWithinTexture;
        public float Rotation;
        public Vector2 Centre;
        public SpriteEffects Effects;
        private int _drawOrder;  // spritebatch depth has 0 at the front to 1 at the back

        public int DrawOrder
            {
            get { return _drawOrder; }
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._drawOrder = value;
                }
            }
        }
    }
