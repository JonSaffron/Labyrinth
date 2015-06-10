using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    abstract class StaticItem
        {
        private Vector2 _position;
        private TilePos _tilePosition;
        private AnimationPlayer _animationPlayer = new AnimationPlayer();
        private readonly World _world;
        private int _energy;

        /// <summary>
        /// Constructs a new static item object
        /// </summary>
        /// <param name="world">A reference to the current world</param>
        /// <param name="position">The initial position of the object</param>
        protected StaticItem(World world, Vector2 position)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (position == null)
                throw new ArgumentNullException("position");

            this._world = world;
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

                const int width = Tile.Width;
                const int height = Tile.Height;
                var left = (int)Math.Round(value.X - (width / 2.0));
                var top = (int)Math.Round(value.Y - (height / 2.0));
                this.BoundingRectangle = new Rectangle(left, top, width, height);
                }
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
        /// Gets a reference to the current world the object inhabits
        /// </summary>
        protected World World 
            { 
            get
                {
                var result = this._world;
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
        /// Creates a new animation player for the object
        /// </summary>
        protected void ResetAnimationPlayerAfterClone()
            {
            this._animationPlayer = new AnimationPlayer();
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
        }
    }
