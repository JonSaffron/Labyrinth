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
    public readonly struct Animation : IEquatable<Animation>
        {
        /// <summary>
        /// Returns a animation that does nothing
        /// </summary>
        public static Animation None { get; } = new Animation();

        /// <summary>
        /// The name of the texture to animate
        /// </summary>
        public readonly string TextureName;

        /// <summary>
        /// Returns the length of the animation, measured in BaseMovements
        /// </summary>
        public readonly int BaseMovesDuringAnimation;

        /// <summary>
        /// Returns the length of the animation, measured in seconds
        /// </summary>
        public readonly float LengthOfAnimation;

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public readonly bool LoopAnimation;

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
        /// <param name="baseMovesDuringAnimation">Specifies the length of the animation in BaseMovements</param>
        public static Animation LoopingAnimation(string textureName, int baseMovesDuringAnimation)
            {
            if (baseMovesDuringAnimation <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovesDuringAnimation));
            var result = new Animation(textureName, baseMovesDuringAnimation, true);
            return result;
            }
        
        /// <summary>
        /// Constructs a new animation which runs only once
        /// </summary>
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovesDuringAnimation">Specifies the length of the animation in BaseMovements</param>
        public static Animation LinearAnimation(string textureName, int baseMovesDuringAnimation)
            {
            if (baseMovesDuringAnimation <= 0)
                throw new ArgumentOutOfRangeException(nameof(baseMovesDuringAnimation));
            var result = new Animation(textureName, baseMovesDuringAnimation, false);
            return result;
            }

        /// <summary>
        /// Constructs a new animation specifying whether it loops
        /// </summary>        
        /// <param name="textureName">The name of a multi-framed graphic image to show</param>
        /// <param name="baseMovesDuringAnimation">Specifies the length of the animation in BaseMovements</param>
        /// <param name="loopAnimation">Specifies whether the animation continually runs or whether it stops on the final frame</param>
        private Animation(string textureName, int baseMovesDuringAnimation, bool loopAnimation)
            {
            this.TextureName = textureName;
            this.BaseMovesDuringAnimation = baseMovesDuringAnimation;
            this.LengthOfAnimation = baseMovesDuringAnimation * Constants.GameClockResolution;
            this.LoopAnimation = loopAnimation;

            this.LengthOfAnimation = 0;
            }

        /// <summary>
        /// Returns whether this animation consists of a single unchanging image
        /// </summary>
        public bool IsStaticAnimation
            {
            get
                {
                var result = this.BaseMovesDuringAnimation == 0;
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
            var result = string.Equals(this.TextureName, other.TextureName) 
                      && this.BaseMovesDuringAnimation == other.BaseMovesDuringAnimation 
                      && this.LoopAnimation == other.LoopAnimation;
            return result;
            }

        /// <summary>
        /// Returns whether this animation object is equivalent to another object
        /// </summary>
        /// <param name="obj">A reference to another object</param>
        /// <returns>true if the other animation is the same object or its values are equivalent</returns>
        public override bool Equals(object obj)
            {
            if (obj == null)
                return false;
            var result = obj is Animation other && this.Equals(other);
            return result;
            }

        public static bool operator ==(Animation a, Animation b)
            {
            return a.Equals(b);
            }

        public static bool operator !=(Animation a, Animation b)
            {
            return !a.Equals(b);
            }

        /// <summary>
        /// Returns a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
            {
            unchecked
                {
                var hashCode = TextureName?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ this.BaseMovesDuringAnimation;
                hashCode = (hashCode * 397) ^ this.LoopAnimation.GetHashCode();
                return hashCode;
                }
            }
        }
    }
