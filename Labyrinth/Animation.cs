using System;
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

        public int BaseMovementsPerFrame { get; private set; }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool LoopAnimation { get; private set; }

        /// <summary>
        /// Constructs a new animation using a static image
        /// </summary>
        /// <param name="world">The world that will provide the graphic content</param>
        /// <param name="textureName">The name of a single framed graphic to show</param>
        public static Animation StaticAnimation(World world, string textureName)
            {
            var result = new Animation(world, textureName, 0, false);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which loops
        /// </summary>
        /// <param name="world">The world that will provide the graphic content</param>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovementsPerFrame">The rate to switch between frames</param>
        public static Animation LoopingAnimation(World world, string textureName, int baseMovementsPerFrame)
            {
            if (baseMovementsPerFrame <= 0)
                throw new ArgumentOutOfRangeException("baseMovementsPerFrame");
            var result = new Animation(world, textureName, baseMovementsPerFrame, true);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which doesn't loop
        /// </summary>
        /// <param name="world">The world that will provide the graphic content</param>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovementsPerFrame">The rate to switch between frames</param>
        public static Animation SingleAnimation(World world, string textureName, int baseMovementsPerFrame)
            {
            if (baseMovementsPerFrame <= 0)
                throw new ArgumentOutOfRangeException("baseMovementsPerFrame");
            var result = new Animation(world, textureName, baseMovementsPerFrame, false);
            return result;
            }

        /// <summary>
        /// Constructs a new animation specifying whether it loops
        /// </summary>        
        private Animation(World world, string textureName, int baseMovementsPerFrame, bool loopAnimation)
            {
            this.Texture = world.LoadTexture(textureName);
            this.BaseMovementsPerFrame = baseMovementsPerFrame;
            this.LoopAnimation = loopAnimation;
            }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
            {
            get { return Texture.Width / Tile.Width; }
            }

        public bool IsStaticAnimation
            {
            get
                {
                var result = this.BaseMovementsPerFrame == 0;
                return result;
                }
            }
        }
    }
