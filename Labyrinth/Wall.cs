using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class Wall : StaticItem
        {
        public Wall(World world, Vector2 position, Texture2D texture) : base(world, position)
            {
            var a = Animation.StaticAnimation(texture);
            this.Ap.PlayAnimation(a);
            }

        public override bool IsExtant
            {
            get
                {
                return true;
                }
            }

        public override ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Impassable;
                }
            }
        }
    }
