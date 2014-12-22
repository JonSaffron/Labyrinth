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
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex { get; set; }

        /// <summary>
        ///  Tracks whether a notification that the animation has completed has been sent
        /// </summary>
        private bool _animationFinishedNotified;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        private double _frameTime;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private double _time;

        /// <summary>
        /// Tracks whether the animation was to be player on the last occasion that Draw was called
        /// </summary>
        private bool _wasMoving;
        
        /// <summary>
        /// Used to notify a game object that the animation has progressed to the next frame
        /// </summary>
        public event EventHandler<EventArgs> NewFrame;
        
        /// <summary>
        /// Used to notify a game object that the animation has completed showing all frames
        /// </summary>
        public event EventHandler<EventArgs> AnimationFinished;
        
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
        
        public static Vector2 Origin
            {
            get { return new Vector2(Tile.Width / 2.0f, Tile.Height / 2.0f); }
            }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
            {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

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
            
            
            if (Animation.IsStaticAnimation)
                {
                // don't do anything
                }
            else
                {
                bool showFirstFrame = !runAnimation || (!this._wasMoving && Animation.LoopAnimation);
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
                }

            _wasMoving = runAnimation;

            // Calculate the source rectangle of the current frame.
            var source = new Rectangle(FrameIndex * Animation.Texture.Width, 0, Animation.Texture.Width, Animation.Texture.Height);

            // Draw the current frame.
            spriteBatch.DrawInWindow(Animation.Texture, position, source, this.Rotation, Origin, this.SpriteEffect);
            }
        }
    }
