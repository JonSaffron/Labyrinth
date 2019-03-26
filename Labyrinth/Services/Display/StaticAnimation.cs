using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    /// <remarks>Only suitable for a animation which is the same size as a tile</remarks>
    public class StaticAnimation : IRenderAnimation
        {
        private readonly IGameObject _gameObject;

        /// <summary>
        /// The animation which is currently playing.
        /// </summary>
        private Animation _animation;

        /// <summary>
        /// What rotation to apply when drawing the sprite
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// What effect to apply when drawing the sprite
        /// </summary>
        public SpriteEffects SpriteEffect { get; set; }

        private double _position;

        /// <summary>
        /// Gets or sets the position within the animation
        /// </summary>
        /// Should be a number equal or above 0 and less than 1
        public double Position
            {
            get => this._position;
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._position = value;
                }
            }

        public StaticAnimation([NotNull] IGameObject gameObject)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._animation = Animation.None;
            }

        /// <summary>
        /// Returns the mid-point of animation
        /// </summary>
        private static Vector2 Origin => Constants.CentreOfTile;

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        /// <remarks>If the animation specified is already running then no change is made.</remarks>
        public void PlayAnimation(Animation animation)
            {
            if (animation == Animation.None)
                throw new ArgumentOutOfRangeException(nameof(animation), "Cannot be None.");

            // If this animation is already running, do not restart it.
            if (animation == this._animation)
                return;

            // Start the new animation.
            this._animation = animation;
            this.Position = 0d;
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            var texture = spriteLibrary.GetSprite(this._animation.TextureName);
            var frameCount = (texture.Width / Constants.TileLength);
            int frameIndex = (this.Position == 1) ? frameCount - 1 : (int) Math.Floor(frameCount * this.Position);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTextureInWindow(texture, this._gameObject.Position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
