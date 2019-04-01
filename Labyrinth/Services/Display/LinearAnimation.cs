using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation that plays from start to finish only once
    /// </summary>
    /// <remarks>Only suitable for a animation which is the same size as a tile</remarks>
    public class LinearAnimation : IRenderAnimation
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
        public float Rotation { get; set; }

        /// <summary>
        /// What effect to apply when drawing the sprite
        /// </summary>
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// The current position within the animation
        /// </summary>
        private double _time;

        public bool HasReachedEndOfAnimation { get; private set; }

        public LinearAnimation([NotNull] IGameObject gameObject, [NotNull] string textureName, int baseMovesDuringAnimation)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._textureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            if (baseMovesDuringAnimation <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovesDuringAnimation));
            this._lengthOfAnimation = baseMovesDuringAnimation * Constants.GameClockResolution;
            }

        public void Update(GameTime gameTime)
            {
            if (this.HasReachedEndOfAnimation) 
                return;

            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            if (this._time >= this._lengthOfAnimation)
                {
                this._time = this._lengthOfAnimation;
                this.HasReachedEndOfAnimation = true;
                }
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(this._textureName);
            var frameCount = (drawParameters.Texture.Width / Constants.TileLength);
            int frameIndex;
            if (this.HasReachedEndOfAnimation)
                frameIndex = frameCount - 1;
            else
                {
                var position = this._time / this._lengthOfAnimation;
                frameIndex = (int) Math.Floor(frameCount * position);
                }

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
