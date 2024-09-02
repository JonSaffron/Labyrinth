using System;
using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    public abstract class StaticItem : IGameObject
        {
        private Vector2 _position;
        private int _energy;

        public PropertyBag Properties { get; } = new PropertyBag();

        /// <summary>
        /// Constructs a new static item object
        /// </summary>
        /// <param name="position">The initial position of the object</param>
        protected StaticItem(Vector2 position)
            {
            this.Position = position;
            }
        
        /// <summary>
        /// Gets or sets the position of the object's centre
        /// </summary>
        public Vector2 Position
            {
            get => this._position;

            protected set
                {
                this._position = value;
                this.TilePosition = TilePos.TilePosFromPosition(value);
                SetBoundingRectangle(value);
                }
            }

        /// <summary>
        /// Gets the nearest tile position to the object's position
        /// </summary>
        public TilePos TilePosition { get; private set; }

        /// <summary>
        /// Updates the position and/or animation of this object each tick of the game
        /// </summary>
        /// <param name="gameTime">The amount of GameTime that has passed since the last time Update was called</param>
        /// <returns>True if the position of the object changes, otherwise false if the object is stationary</returns>
        public virtual bool Update(GameTime gameTime)
            {
            return false;
            }

        /// <summary>
        /// Returns an object that animates the GameObject. Can be null if the object is not visible, or does not have a physical presence.
        /// </summary>
        public abstract IRenderAnimation? RenderAnimation { get; }

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
                    throw new ArgumentOutOfRangeException(nameof(value));
                this._energy = value;
                }
            }
        
        /// <summary>
        /// Gets the boundaries of the game object's position for basic collision detection purposes
        /// </summary>
        public virtual Rectangle BoundingRectangle { get; protected set; }

        /// <summary>
        /// Removes the specified amount of energy from the object
        /// </summary>
        /// <param name="energyToRemove">The amount to reduce the object's energy by</param>
        public virtual void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException(nameof(energyToRemove), energyToRemove, "Must be above 0.");
            if (!this.IsExtant)
                return;
            
            this.Energy = (this.Energy > energyToRemove) ? this.Energy - energyToRemove : 0;
            if (this.IsExtant)
                UponInjury();
            else
                UponDeath(wasDeathInstant: false);
            }

        /// <summary>
        /// Instantly reduce the object's energy to zero
        /// </summary>
        public virtual void InstantlyExpire()
            {
            if (!this.IsExtant)
                return;
            
            this.Energy = 0;
            UponDeath(wasDeathInstant: true);
            }

        /// <summary>
        /// Override this method to implement any behaviour that needs to happen when this object is injured
        /// </summary>
        protected virtual void UponInjury()
            {
            // override as necessary
            }

        /// <summary>
        /// Override this method to implement any behaviour that needs to happen when the object no longer has any energy remaining
        /// </summary>
        /// <param name="wasDeathInstant">When set to true it indicates that the object was outright destroyed, otherwise it was an injury that did for it</param>
        protected virtual void UponDeath(bool wasDeathInstant)
            {
            // override as necessary
            }

        /// <summary>
        /// Gets whether the object has any energy remaining
        /// </summary>
        public virtual bool IsExtant => this.Energy > 0;

        /// <summary>
        /// Plays a sound which is centred on this instance
        /// </summary>
        /// <param name="gameSound">Sets which sound to play</param>
        public void PlaySound(GameSound gameSound)
            {
            GlobalServices.SoundPlayer.PlayForObject(gameSound, this, GlobalServices.CentrePointProvider);
            }

        private void SetBoundingRectangle(Vector2 position)
            {
            var r = Constants.TileRectangle;
            var offsetX = (int) position.X - Constants.HalfTileLength;
            var offsetY = (int) position.Y - Constants.HalfTileLength;
            r.Offset(offsetX, offsetY);
            this.BoundingRectangle = r;
            }
        }
    }
