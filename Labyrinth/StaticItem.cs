using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    abstract class StaticItem
        {
        protected readonly AnimationPlayer Ap = new AnimationPlayer();
        
        public int Energy { get; protected set; }
        
        public Vector2 Position { get; protected set; }

        public World World { get; private set; }
        
        protected StaticItem(World world, Vector2 position)
            {
            this.World = world;
            this.Position = position;
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
        
        /// <summary>
        /// Determines what happens when the player touches this object
        /// </summary>
        /// <param name="p">An instance of the player object</param>
        /// <returns>Whether to remove this object or not</returns>
        public virtual TouchResult OnTouched(Player p)
            {
            return TouchResult.NoEffect;
            }
        
        /// <summary>
        /// Determines what happens when a shot touches this object
        /// </summary>
        /// <param name="s">An instance of a shot object</param>
        /// <returns>Whether the shot hit this object, bounced off it, or has no effect on it</returns>
        public virtual ShotStatus OnShot(Shot s)
            {
            if (!IsExtant)
                return ShotStatus.CarryOn;
            
            this.World.Game.SoundLibrary.Play(GameSound.StaticObjectShotAndInjured);
            this.ReduceEnergy(s.Energy);
            return ShotStatus.HitHome;
            }

        public virtual void ReduceEnergy(int energy)
            {
            if (energy <= 0)
                throw new ArgumentOutOfRangeException("energy", energy, "Must be above 0.");
            this.Energy -= energy;
            if (this.Energy < 0)
                {
                this.Energy = 0;
                }
            }

        public virtual bool IsExtant
            {
            get
                {
                return this.Energy > 0;
                }
            }
        }
    }
