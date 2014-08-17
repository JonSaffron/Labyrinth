using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    class Animation
        {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture { get; private set; }

        public int BaseMovementsPerFrame { get; private set;}

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool LoopAnimation { get; private set; }

        /// <summary>
        /// Constructs a new animation using a static image
        /// </summary>
        /// <param name="texture"></param>
        public static Animation StaticAnimation(Texture2D texture)
            {
            var result = new Animation(texture, 0, false);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which loops
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseMovementsPerFrame"></param>
        public static Animation LoopingAnimation(Texture2D texture, int baseMovementsPerFrame)
            {
            var result = new Animation(texture, baseMovementsPerFrame, true);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which doesn't loop
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseMovementsPerFrame"></param>
        public static Animation SingleAnimation(Texture2D texture, int baseMovementsPerFrame)
            {
            var result = new Animation(texture, baseMovementsPerFrame, false);
            return result;
            }

        /// <summary>
        /// Constructs a new animation specifying whether it loops
        /// </summary>        
        private Animation(Texture2D texture, int baseMovementsPerFrame, bool loopAnimation)
            {
            this.Texture = texture;
            this.BaseMovementsPerFrame = baseMovementsPerFrame;
            this.LoopAnimation = loopAnimation;
            }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
            {
            get { return Texture.Width / FrameWidth; }
            }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
            {
            // Assume square frames.
            get { return Texture.Height; }
            }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
            {
            get { return Texture.Height; }
            }
        
        public bool IsStaticAnimation
            {
            get
                {
                var result = this.BaseMovementsPerFrame == 0;
                return result;
                }
            }

        public static Vector2 GetBaseVectorForDirection(Direction d)
            {
            switch (d)
                {
                case Direction.Left: 
                    return new Vector2(-1, 0);
                case Direction.Right:
                    return new Vector2(1, 0);
                case Direction.Up:
                    return new Vector2(0, -1);
                case Direction.Down:
                    return new Vector2(0, 1);
                case Direction.None:
                    return new Vector2(0, 0);
                default:
                    throw new InvalidOperationException();
                }
            }
        }
    }
