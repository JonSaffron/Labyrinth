using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Wall : StaticItem
        {
        public Wall(World world, Vector2 position, string textureName) : base(world, position)
            {
            var a = Animation.StaticAnimation(World, textureName);
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

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Wall;
                }
            }
        }
    }
