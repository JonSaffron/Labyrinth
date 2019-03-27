﻿using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    /// <remarks>Only suitable for a animation which is the same size as a tile</remarks>
    public class LoopedAnimation : IRenderAnimation
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

        public LoopedAnimation([NotNull] IGameObject gameObject, [NotNull] string textureName, int baseMovesDuringAnimation)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._textureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            if (baseMovesDuringAnimation <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovesDuringAnimation));
            this._lengthOfAnimation = baseMovesDuringAnimation * Constants.GameClockResolution;
            }

        /// <summary>
        /// Returns the mid-point of animation
        /// </summary>
        private static Vector2 Origin => Constants.CentreOfTile;

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

            var texture = spriteLibrary.GetSprite(this._textureName);
            var frameCount = (texture.Width / Constants.TileLength);
            int frameIndex = (int) Math.Floor(frameCount * positionInAnimation);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTextureInWindow(texture, this._gameObject.Position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
