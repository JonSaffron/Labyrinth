using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Grave : StaticItem
        {
        public Grave(World world, Vector2 position) : base(world, position)
            {
            this.Energy = 255;
            
            var a = Animation.StaticAnimation(World, "Sprites/Props/Grave");
            this.Ap.PlayAnimation(a);
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.StaticItem;
                }
            }
        }
    }
