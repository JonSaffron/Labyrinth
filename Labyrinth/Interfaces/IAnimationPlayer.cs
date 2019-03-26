using System;
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

        /// <summary>
        /// Gets or sets the position within the animation
        /// </summary>
        /// Should be a number equal or above 0 and less than 1
        double Position { get; set; }

        /// <summary>
        /// Used to notify a game object that the animation has progressed to the next frame
        /// </summary>
        event EventHandler<EventArgs> NewFrame;

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        /// <remarks>If the animation specified is already running then no change is made.</remarks>
        void PlayAnimation(Animation animation);

        /// <summary>
        /// Updates the state of the animation
        /// </summary>
        /// <param name="gameTime">The amount of time passed</param>
        void Update(GameTime gameTime);
        }
    }
