using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation that loops repeatedly
    /// </summary>
    /// <remarks>Only suitable for an animation which is the same size as a tile</remarks>
    public class LoopedAnimation : IDynamicAnimation
        {
        private readonly IGameObject _gameObject;

        /// <summary>
        /// The name of the texture to render
        /// </summary>
        private readonly string _textureName;

        /// <summary>
        /// How long it takes to play the whole animation in seconds
        /// </summary>
        private readonly float _lengthOfAnimation;

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
        /// The opacity of the drawn image
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// The current position within the animation
        /// </summary>
        private double _time;

        public LoopedAnimation(IGameObject gameObject, string textureName, int baseMovesDuringAnimation)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._textureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            if (baseMovesDuringAnimation <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovesDuringAnimation));
            this._lengthOfAnimation = baseMovesDuringAnimation * Constants.GameClockResolution;
            this.Opacity = 1f;
            }

        public void Update(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            this._time %= this._lengthOfAnimation;
            var positionInAnimation = this._time / this._lengthOfAnimation;

            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(this._textureName);
            var frameCount = (drawParameters.Texture.Width / Constants.TileLength);
            int frameIndex = (int) Math.Floor(frameCount * positionInAnimation);

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);
            drawParameters.Position = this._gameObject.Position;
            drawParameters.Rotation = this.Rotation;
            drawParameters.Effects = this.SpriteEffect;
            drawParameters.Opacity = this.Opacity;

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }
        }
    }
