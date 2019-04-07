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
        private float? _opacity;
        public float Rotation;
        private Vector2? _centre;
        public SpriteEffects Effects;
        private int _drawOrder;  // spritebatch depth has 0 at the front to 1 at the back

        public float Opacity
            {
            get => this._opacity ?? 1f;
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._opacity = value;
                }
            }

        public Vector2 Centre
            {
            get => _centre ?? Constants.CentreOfTile;
            set => _centre = value;
            }

        public int DrawOrder
            {
            get => _drawOrder;
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._drawOrder = value;
                }
            }
        }
    }
