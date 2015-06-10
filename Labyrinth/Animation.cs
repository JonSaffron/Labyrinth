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
    class Animation : IEquatable<Animation>
        {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Returns the rate at which the animation moves to the next frame. 
        /// The higher the number, the longer the animation will remain on each frame.
        /// </summary>
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
        /// Constructs a new animation which runs through only once
        /// </summary>
        /// <param name="world">The world that will provide the graphic content</param>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovementsPerFrame">The rate to switch between frames</param>
        public static Animation SingleRunAnimation(World world, string textureName, int baseMovementsPerFrame)
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

        /// <summary>
        /// Returns whether this animation consists of a single unchanging image
        /// </summary>
        public bool IsStaticAnimation
            {
            get
                {
                var result = this.BaseMovementsPerFrame == 0;
                return result;
                }
            }

        /// <summary>
        /// Returns whether this animation object is equivalent to another
        /// </summary>
        /// <param name="other">A reference to another animation object</param>
        /// <returns>true if the other animation is the same object or its values are equivalent</returns>
        public bool Equals(Animation other)
            {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var result = Equals(Texture, other.Texture) && BaseMovementsPerFrame == other.BaseMovementsPerFrame && LoopAnimation.Equals(other.LoopAnimation);
            return result;
            }

        /// <summary>
        /// Returns whether this animation object is equivalent to another object
        /// </summary>
        /// <param name="obj">A reference to another object</param>
        /// <returns>true if the other animation is the same object or its values are equivalent</returns>
        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            var result = this.Equals((Animation) obj);
            return result;
            }

        /// <summary>
        /// Returns a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
            {
            unchecked
                {
                int hashCode = (Texture != null ? Texture.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ BaseMovementsPerFrame;
                hashCode = (hashCode*397) ^ LoopAnimation.GetHashCode();
                return hashCode;
                }
            }
        }
    }
