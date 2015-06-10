using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    class AnimationPlayer
        {
        // Number of base movements a sprite can make per second
        private const int MovementsPerSecond = 20;

        // Smallest move a sprite can make is a quarter of a tile
        private const int BaseDistance = Tile.Width / 4;

        // Slowest move a sprite can make is a quarter of a tile, and 20 movements take place per second.
        // Unit of measurement is therefore pixels per second.
        public const int BaseSpeed = BaseDistance * MovementsPerSecond;

        // Speed when bouncing back
        public const int BounceBackSpeed = BaseSpeed * 3;

        // The internal game clock ticks once every movement by a sprite
        // Unit of measurement is seconds.
        public const float GameClockResolution = 1.0f / MovementsPerSecond;

        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation { get; private set; }

        /// <summary>
        /// What rotation to apply when drawing the sprite
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// What effect to apply when drawing the sprite
        /// </summary>
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Records the index of the current frame in the animation.
        /// </summary>
        private int _frameIndex;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        private double _frameTime;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private double _time;

        /// <summary>
        /// Used to notify a game object that the animation has progressed to the next frame
        /// </summary>
        public event EventHandler<EventArgs> NewFrame;
        
        /// <summary>
        /// Used to notify a game object that the animation has completed showing all frames
        /// </summary>
        public event EventHandler<EventArgs> AnimationFinished;
        
        /// <summary>
        /// Which routine to use to advance the frame index
        /// </summary>
        private Action<GameTime> _advanceRoutine;

        private void OnNewFrame(EventArgs e)
            {
            var handler = NewFrame;

            if (handler != null)
                handler(this, e);
            }
        
        private void OnAnimationFinished(EventArgs e)
            {
            var handler = AnimationFinished;

            if (handler != null)
                handler(this, e);
            }
        
#warning this needs to be refactored - not all sprites will be tile sized.
        /// <summary>
        /// Returns the mid-point of animation
        /// </summary>
        private static Vector2 Origin
            {
            get
                {
                var result = Tile.Size / 2;
                return result;
                }
            }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
            {
            if (animation == null)
                throw new ArgumentNullException("animation");

            // If this animation is already running, do not restart it.
            if (animation.Equals(this.Animation))
                return;

            // Start the new animation.
            this.Animation = animation;
            this._frameTime = animation.BaseMovementsPerFrame * BaseDistance / (double)BaseSpeed;
            this._frameIndex = 0;
            this._time = 0.0f;

            // setup the advance routine
            if (animation.IsStaticAnimation)
                this._advanceRoutine = AdvanceStaticAnimation;
            else if (animation.LoopAnimation)
                this._advanceRoutine = AdvanceLoopedAnimation;
            else
                this._advanceRoutine = AdvanceSingleRunAnimation;
            }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex
            {
            get
                {
                return this._frameIndex;
                }
            set
                {
                if (value < 0 || value >= Animation.FrameCount)
                    throw new ArgumentOutOfRangeException("value");
                this._frameIndex = value;
                this._time = 0;
                }
            }

        /// <summary>
        /// Advance frame index for static animations
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        private static void AdvanceStaticAnimation(GameTime gameTime)
            {
            // nothing to do
            }

        /// <summary>
        /// Advance frame index for looped animations
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        private void AdvanceLoopedAnimation(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time >= this._frameTime)
                {
                this._time -= this._frameTime;
                OnNewFrame(new EventArgs());

                this._frameIndex++;
                this._frameIndex %= Animation.FrameCount;
                }
            }

        /// <summary>
        /// Advance frame index for single run animations
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        private void AdvanceSingleRunAnimation(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time >= this._frameTime)
                {
                this._time -= this._frameTime;
                OnNewFrame(new EventArgs());

                if ((this._frameIndex + 1) == Animation.FrameCount)
                    {
                    OnAnimationFinished(new EventArgs());
                    this._advanceRoutine = AdvanceStaticAnimation;
                    return;
                    }

                this._frameIndex++;
                }
            }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        /// <param name="spriteBatch">The SpriteBatch object to draw the sprite to</param>
        /// <param name="position">The position of the sprite</param>
        public void Draw(GameTime gameTime, ISpriteBatch spriteBatch, Vector2 position)
            {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");
            
            // Advance the frame index
            this._advanceRoutine(gameTime);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(FrameIndex * Tile.Width, 0, Tile.Width, Tile.Height);

            // Draw the current frame.
            spriteBatch.DrawTexture(Animation.Texture, position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
