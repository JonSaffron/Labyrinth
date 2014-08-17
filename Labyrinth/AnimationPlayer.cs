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
        /// Gets the index of the current frame in the animation.
        /// </summary>
        private int FrameIndex { get; set; }

        private bool _animationFinishedNotified;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        private double _frameTime;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private double _time;

        private bool _wasMoving;
        
        public delegate void NewFrameHandler(object sender, EventArgs e);
        public event NewFrameHandler NewFrame;
        
        public delegate void AnimationFinishedHandler(object sender, EventArgs e);
        public event AnimationFinishedHandler AnimationFinished;
        
        private void OnNewFrame(EventArgs e)
            {
            if (NewFrame != null)
                NewFrame(this, e);
            }
        
        private void OnAnimationFinished(EventArgs e)
            {
            if (AnimationFinished != null)
                AnimationFinished(this, e);
            }
        
        public Vector2 Origin
            {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight / 2.0f); }
            }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
            {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            if (animation.FrameCount > 1 && animation.BaseMovementsPerFrame == 0)
                throw new InvalidOperationException("Cannot play a multi-framed animation with a frametime of 0.");

            // Start the new animation.
            this.Animation = animation;
            this._frameTime = animation.BaseMovementsPerFrame * BaseDistance / (double)BaseSpeed;
            this.FrameIndex = 0;
            this._time = 0.0f;
            }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatchWindowed spriteBatch, Vector2 position, bool runAnimation = true)
            {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");
            
            bool showFirstFrame = Animation.IsStaticAnimation || !runAnimation || (!this._wasMoving && Animation.LoopAnimation);
            if (showFirstFrame)
                {
                FrameIndex = 0;
                _time = 0;
                }
            else
                {
                // Process passing time.
                _time += gameTime.ElapsedGameTime.TotalSeconds;
                while (_time > this._frameTime)
                    {
                    _time -= this._frameTime;
                    OnNewFrame(new EventArgs());

                    // Advance the frame index; looping or clamping as appropriate.
                    if (Animation.LoopAnimation)
                        {
                        FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                        }
                    else 
                        {
                        if (FrameIndex == (Animation.FrameCount - 1))
                            {
                            if (!_animationFinishedNotified)
                                {
                                OnAnimationFinished(new EventArgs());
                                _animationFinishedNotified = true;
                                }
                            }
                        else
                            FrameIndex++;                        
                        }
                    }
                }

            _wasMoving = runAnimation;

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            spriteBatch.DrawInWindow(Animation.Texture, position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
