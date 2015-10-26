using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Wall : StaticItem
        {
        public Wall(AnimationPlayer animationPlayer, Vector2 position, string textureName) : base(animationPlayer, position)
            {
            var a = Animation.StaticAnimation(textureName);
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
