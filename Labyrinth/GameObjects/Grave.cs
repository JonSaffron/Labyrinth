using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Grave : StaticItem
        {
        public Grave(World world, Vector2 position) : base(world, position)
            {
            this.Energy = 255;
            
            var a = Animation.StaticAnimation("Sprites/Props/Grave");
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
