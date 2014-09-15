using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    abstract class StaticItem
        {
        private AnimationPlayer _animationPlayer = new AnimationPlayer();
        
        public int Energy { get; protected set; }
        
        public Vector2 Position { get; protected set; }

        public World World { get; private set; }
        
        protected StaticItem(World world, Vector2 position)
            {
            this.World = world;
            this.Position = position;
            }
        
        protected AnimationPlayer Ap
            {
            get
                {
                var result = this._animationPlayer;
                return result;
                }
            }

        protected void ResetAnimationPlayerAfterClone()
            {
            this._animationPlayer = new AnimationPlayer();
            }

        public virtual Rectangle BoundingRectangle
            {
            get
                {
                var left = (int)Math.Round(Position.X - Ap.Origin.X);
                var top = (int)Math.Round(Position.Y - Ap.Origin.Y);

                return new Rectangle(left, top, Ap.Animation.FrameWidth, Ap.Animation.FrameHeight);
                }
            }

        public virtual void Draw(GameTime gt, SpriteBatchWindowed spriteBatch)
            {
            if (IsExtant)
                Ap.Draw(gt, spriteBatch, Position);
            }
        
        public virtual void ReduceEnergy(int energyToRemove)
            {
            if (energyToRemove <= 0)
                throw new ArgumentOutOfRangeException("energyToRemove", energyToRemove, "Must be above 0.");

            this.Energy = (this.Energy > energyToRemove) ? this.Energy - energyToRemove : 0;
            }

        public virtual bool IsExtant
            {
            get
                {
                return this.Energy > 0;
                }
            }

        public virtual ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Passable;
                }
            }
        }
    }
