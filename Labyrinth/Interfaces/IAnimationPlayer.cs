using System;
using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface IAnimationPlayer
        {
        /// <summary>
        /// What rotation to apply when drawing the sprite
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// What effect to apply when drawing the sprite
        /// </summary>
        SpriteEffects SpriteEffect { get; set; }

        int FrameCount { get; }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        int FrameIndex { get; set; }

        /// <summary>
        /// Used to notify a game object that the animation has progressed to the next frame
        /// </summary>
        event EventHandler<EventArgs> NewFrame;

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        void PlayAnimation([NotNull] Animation animation);

        /// <summary>
        /// Advance frame index for manually controlled animations
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        bool AdvanceManualAnimation(GameTime gameTime);

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        /// <param name="spriteBatch">The SpriteBatch object to draw the sprite to</param>
        /// <param name="position">The position of the sprite</param>
        void Draw(GameTime gameTime, ISpriteBatch spriteBatch, Vector2 position);
        }
    }