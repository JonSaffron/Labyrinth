using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Renders a GameObject with a manually selected image
    /// </summary>
    /// <remarks>Only suitable for an animation which is the same size as a tile</remarks>
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
        [PublicAPI]
        public float Rotation { get; set; }

        /// <summary>
        /// What effects to apply when drawing the sprite
        /// </summary>
        [PublicAPI]
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

        public StaticAnimation(IGameObject gameObject, string textureName)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._textureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(this._textureName);
            var frameCount = (drawParameters.Texture.Width / Constants.TileLength);
            int frameIndex = (this.Position == 1) ? frameCount - 1 : (int) Math.Floor(frameCount * this.Position);

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);
            drawParameters.Position = this._gameObject.Position;
            drawParameters.Rotation = this.Rotation;
            drawParameters.Effects = this.SpriteEffect;

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }
        }
    }
