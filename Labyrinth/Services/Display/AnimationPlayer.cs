using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    /// <remarks>Only suitable for a animation which is the same size as a tile</remarks>
    public class AnimationPlayer : IAnimationPlayer, IRenderAnimation
        {
        private readonly IGameObject _gameObject;

        /// <summary>
        /// The animation which is currently playing.
        /// </summary>
        private Animation _animation;

        /// <inheritdoc />
        public float Rotation { get; set; }

        /// <inheritdoc />
        public SpriteEffects SpriteEffect { get; set; }

        private double _position;

        /// <inheritdoc />
        public double Position
            {
            get => this._position;
            set
                {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._position = value;
                }
            }

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

        public AnimationPlayer([NotNull] IGameObject gameObject)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            this._animation = Animation.None;
            }

        private void OnNewFrame(EventArgs e)
            {
            this.NewFrame?.Invoke(this, e);
            }
        
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
            this._time = 0d;
            this.Position = 0d;

            // setup the advance routine
            if (!animation.IsStaticAnimation)
                {
                this._advanceRoutine = this._animation.LoopAnimation
                    ? (Action<GameTime>) AdvanceLoopedAnimation
                    : AdvanceLinearAnimation;
                }
            }

        /// <summary>
        /// Advance frame index for looped animations
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw</param>
        private void AdvanceLoopedAnimation(GameTime gameTime)
            {
            this._time = (this._time + gameTime.ElapsedGameTime.TotalSeconds) % this._animation.LengthOfAnimation;
            this.Position = this._time / this._animation.LengthOfAnimation;
            // todo trigger OnNewFrame
            }

        private void AdvanceLinearAnimation(GameTime gameTime)
            {
            if (this._time < this._animation.LengthOfAnimation)
                {
                this._time += gameTime.ElapsedGameTime.TotalSeconds;
                if (this._time < this._animation.LengthOfAnimation)
                    {
                    this.Position = this._time / this._animation.LengthOfAnimation;
                    }
                else
                    {
                    this._time = this._animation.LengthOfAnimation;
                    this._position = 1;
                    }
                }
            // todo trigger OnNewFrame
            }

        public void Update(GameTime gameTime)
            {
            if (this._animation == Animation.None)
                throw new InvalidOperationException("No animation is currently playing.");

            // Advance the frame index
            this._advanceRoutine?.Invoke(gameTime);
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._gameObject.IsExtant)
                return;

            var texture = spriteLibrary.GetSprite(this._animation.TextureName);
            var frameCount = (texture.Width / Constants.TileLength);
            int frameIndex = (this.Position == 1) ? frameCount - 1 : (int) Math.Floor(frameCount * this.Position);

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTextureInWindow(texture, this._gameObject.Position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
