using System;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    public class Animation : IEquatable<Animation>
        {
        /// <summary>
        /// The name of the texture to animate
        /// </summary>
        public string TextureName { get; }

        /// <summary>
        /// Returns the rate at which the animation moves to the next frame. 
        /// The higher the number, the longer the animation will remain on each frame.
        /// </summary>
        public int BaseMovementsPerFrame { get; }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool LoopAnimation { get; }

        /// <summary>
        /// Constructs a new animation using a static image
        /// </summary>
        /// <param name="textureName">The name of a single framed graphic to show</param>
        public static Animation StaticAnimation(string textureName)
            {
            var result = new Animation(textureName, 0, false);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which loops
        /// </summary>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovementsPerFrame">The rate to switch between frames</param>
        public static Animation LoopingAnimation(string textureName, int baseMovementsPerFrame)
            {
            if (baseMovementsPerFrame <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovementsPerFrame));
            var result = new Animation(textureName, baseMovementsPerFrame, true);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which can be manually controlled
        /// </summary>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovementsPerFrame">The rate to switch between frames</param>
        public static Animation ManualAnimation(string textureName, int baseMovementsPerFrame)
            {
            if (baseMovementsPerFrame <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovementsPerFrame));
            var result = new Animation(textureName, baseMovementsPerFrame, false);
            return result;
            }

        /// <summary>
        /// Constructs a new animation specifying whether it loops
        /// </summary>        
        private Animation(string textureName, int baseMovementsPerFrame, bool loopAnimation)
            {
            this.TextureName = textureName;
            this.BaseMovementsPerFrame = baseMovementsPerFrame;
            this.LoopAnimation = loopAnimation;
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
            var result = string.Equals(TextureName, other.TextureName) && BaseMovementsPerFrame == other.BaseMovementsPerFrame && LoopAnimation.Equals(other.LoopAnimation);
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
                int hashCode = (TextureName != null ? TextureName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ BaseMovementsPerFrame;
                hashCode = (hashCode*397) ^ LoopAnimation.GetHashCode();
                return hashCode;
                }
            }
        }
    }
