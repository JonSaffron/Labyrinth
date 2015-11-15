using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class StaticItem : IGameObject
        {
        private Vector2 _position;
        private TilePos _tilePosition;
        private readonly AnimationPlayer _animationPlayer;
        private int _energy;

        /// <summary>
        /// Constructs a new static item object
        /// </summary>
        /// <param name="animationPlayer">An instance of the animation player to use for animating this object</param>
        /// <param name="position">The initial position of the object</param>
        protected StaticItem(AnimationPlayer animationPlayer, Vector2 position)
            {
            if (animationPlayer == null)
                throw new ArgumentNullException("animationPlayer");
            if (position == null)
                throw new ArgumentNullException("position");

            this._animationPlayer = animationPlayer;
            this.Position = position;
            }
        
        /// <summary>
        /// Gets or sets the position of the object
        /// </summary>
        public Vector2 Position
            {
            get
                {
                return this._position;
                }

            protected set
                {
                this._position = value;
                this._tilePosition = TilePos.TilePosFromPosition(value);
                SetBoundingRectangle(value);
                }
            }

        private void SetBoundingRectangle(Vector2 position)
            {
            const int width = Constants.TileLength;
            const int height = Constants.TileLength;
            var left = (int)Math.Round(position.X - (width / 2.0));
            var top = (int)Math.Round(position.Y - (height / 2.0));
            this.BoundingRectangle = new Rectangle(left, top, width, height);
            }

        /// <summary>
        /// Gets the nearest tile position to the object's position
        /// </summary>
        public TilePos TilePosition
            {
            get
                {
                return this._tilePosition;
                }
            }

        /// <summary>
        /// Gets the animation player associated with the object
        /// </summary>
        protected AnimationPlayer Ap
            {
            get
                {
                var result = this._animationPlayer;
                return result;
                }
            }

        /// <summary>
        /// Gets or sets how much energy the object has
        /// </summary>
        public int Energy 
            {
            get
                {
                var result = this._energy;
                return result;
                }

            protected set
                {
                if (value < 0 || value > 255)
                    throw new ArgumentOutOfRangeException("value");
                this._energy = value;
                }
            }
        
        /// <summary>
        /// Gets the boundaries of the game object's position for basic collision detection purposes
        /// </summary>
        public virtual Rectangle BoundingRectangle { get; protected set; }

        /// <summary>
        /// Draws the object if it has any energy remaining
        /// </summary>
        /// <param name="gt">The current gametime</param>
        /// <param name="spriteBatch">The spritebatch to draw to</param>
        public virtual void Draw(GameTime gt, ISpriteBatch spriteBatch)
            {
            if (this.IsExtant)
                this.Ap.Draw(gt, spriteBatch, this.Position);
            }
        
        /// <summary>
        /// Removes the specified amount of energy from the object
        /// </summary>
        /// <param name="energyToRemove">The amount to reduce the object's energy by</param>
        public virtual void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException("energyToRemove", energyToRemove, "Must be above 0.");

            this.Energy = (this.Energy > energyToRemove) ? this.Energy - energyToRemove : 0;
            }

        /// <summary>
        /// Reduce the object's energy to zero
        /// </summary>
        /// <returns>The amount of energy the object had before it expired</returns>
        public virtual int InstantlyExpire()
            {
            if (!this.IsExtant)
                return 0;
            
            int result = this.Energy;
            this.Energy = 0;
            return result;
            }

        /// <summary>
        /// Gets whether the object has any energy remaining
        /// </summary>
        public virtual bool IsExtant
            {
            get
                {
                return this.Energy > 0;
                }
            }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        public virtual ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Stationary;
                }
            }

        /// <summary>
        /// Gets the order that objects are drawn in (lowest before highest)
        /// </summary>
        public abstract int DrawOrder { get; }

        public void PlaySound(GameSound gameSound)
            {
            GlobalServices.SoundPlayer.PlayForObject(gameSound, this, GlobalServices.CentrePointProvider);
            }

        public void PlaySoundWithCallback(GameSound gameSound, EventHandler callback)
            {
            GlobalServices.SoundPlayer.PlayForObjectWithCallback(gameSound, this, GlobalServices.CentrePointProvider, callback);
            }
        }
    }
