using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    internal abstract class MovingItem : StaticItem
        {
        public Direction Direction { get; set;}
        protected float CurrentVelocity { get; set; }
        public Vector2 MovingTowards { get; protected set; }

        public abstract void Update(GameTime gameTime);

        protected MovingItem(World world, Vector2 position) : base(world, position)
            {
            this.MovingTowards = position;
            }
        }
    }
