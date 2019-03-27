using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Defines a static image or one that can be altered manually
    /// </summary>
    /// <remarks>Only suitable for a animation which is the same size as a tile</remarks>
    public class StaticAnimation : IRenderAnimation
        {
        private readonly IGameObject _gameObject;
        private decimal _position;

        /// <summary>
        /// The name of the texture to render
        /// </summary>
        private readonly string _textureName;

        /// <summary>
        /// What rotation to apply when drawing the sprite
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// What effect to apply when drawing the sprite
        /// </summary>
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Gets or sets the position within the animation
        /// </summary>
        /// Should be a number equal or above 0 and less than or equal to 1
        public decimal Position
            {
            get => this._position;
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._position = value;
                }
            }

        public StaticAnimation([NotNull] IGameObject gameObject, [NotNull] string textureName)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._textureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            }

        /// <summary>
        /// Returns the mid-point of animation
        /// </summary>
        private static Vector2 Origin => Constants.CentreOfTile;

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            var texture = spriteLibrary.GetSprite(this._textureName);
            var frameCount = (texture.Width / Constants.TileLength);
            int frameIndex = (this.Position == 1) ? frameCount - 1 : (int) Math.Floor(frameCount * this.Position);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTextureInWindow(texture, this._gameObject.Position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
