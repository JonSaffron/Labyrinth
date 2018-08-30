using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public class AnimationPlayer : IAnimationPlayer
        {
        /// <summary>
        /// The animation which is currently playing.
        /// </summary>
        private Animation _animation;

        private readonly ISpriteLibrary _spriteLibrary;

        private Texture2D _texture;

        /// <inheritdoc />
        public float Rotation { get; set; }

        /// <inheritdoc />
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Records the index of the current frame in the animation.
        /// </summary>
        private int _frameIndex;

        /// <inheritdoc />
        public int FrameCount {get; private set;}

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        private double _frameTime;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private double _time;

        /// <inheritdoc />
        public event EventHandler<EventArgs> NewFrame;
        
        /// <summary>
        /// Which routine to use to advance the frame index
        /// </summary>
        private Action<GameTime> _advanceRoutine;

        public AnimationPlayer([NotNull] ISpriteLibrary spriteLibrary)
            {
            this._spriteLibrary = spriteLibrary ?? throw new ArgumentNullException(nameof(spriteLibrary));
            }

        private void OnNewFrame(EventArgs e)
            {
            this.NewFrame?.Invoke(this, e);
            }
        
//todo this needs to be refactored - not all sprites will be tile sized.
        /// <summary>
        /// Returns the mid-point of animation
        /// </summary>
        private static Vector2 Origin => Constants.CentreOfTile;

        /// <inheritdoc />
        public void PlayAnimation(Animation animation)
            {
            if (animation == Animation.None)
                throw new ArgumentOutOfRangeException(nameof(animation), "Cannot be None.");

            // If this animation is already running, do not restart it.
            if (animation == this._animation)
                return;

            // Start the new animation.
            this._animation = animation;
            this._texture = this._spriteLibrary.GetSprite(animation.TextureName);
            this._frameTime = animation.BaseMovementsPerFrame * Constants.BaseDistance / (double)Constants.BaseSpeed;
            this._frameIndex = 0;
            this.FrameCount = (this._texture.Width / Constants.TileLength);
            this._time = 0.0f;

            // setup the advance routine
            if (animation.IsStaticAnimation)
                this._advanceRoutine = AdvanceStaticAnimation;
            else if (animation.LoopAnimation)
                this._advanceRoutine = AdvanceLoopedAnimation;
            }

        /// <inheritdoc />
        public int FrameIndex
            {
            get => this._frameIndex;
            set
                {
                if (value < 0 || value >= this.FrameCount)
                    throw new ArgumentOutOfRangeException(nameof(value));
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
                this._frameIndex %= this.FrameCount;
                }
            }

        /// <inheritdoc />
        public bool AdvanceManualAnimation(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time >= this._frameTime)
                {
                this._time -= this._frameTime;
                OnNewFrame(new EventArgs());

                if ((this._frameIndex + 1) == this.FrameCount)
                    {
                    return false;
                    }

                this._frameIndex++;
                }
            return true;
            }

        /// <inheritdoc />
        public void Draw(GameTime gameTime, ISpriteBatch spriteBatch, Vector2 position)
            {
            if (this._animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Advance the frame index
            this._advanceRoutine?.Invoke(gameTime);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(FrameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTextureInWindow(this._texture, position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
