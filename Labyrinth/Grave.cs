using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class Grave : StaticItem
        {
        public Grave(World world, Vector2 position) : base(world, position)
            {
            this.Energy = 255;
            
            var texture = base.World.Content.Load<Texture2D>("Sprites/Props/Grave");
            var a = Animation.StaticAnimation(texture);
            this.Ap.PlayAnimation(a);
            }
        }
    }
