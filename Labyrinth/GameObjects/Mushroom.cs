using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Mushroom : StaticItem
        {
        private bool _isTaken;

        public Mushroom(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            var a = Animation.StaticAnimation("Sprites/Props/Mushroom");
            this.Ap.PlayAnimation(a);
            this.Energy = 40;
            }
        
        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.StaticItem;
                }
            }

        public void SetTaken()
            {
            this._isTaken = true;
            }

        public int CalculateEnergyToRemove(Player p)
            {
            if (!this.IsExtant)
                return 0;

            var result = p.Energy >> 2;
            return result;
            }
        }
    }
