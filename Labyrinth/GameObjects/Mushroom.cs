using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Mushroom : StaticItem
        {
        private bool _isTaken;

        public Mushroom(World world, Vector2 position) : base(world, position)
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

        public int CalculateEnergyToRemove(StaticItem si)
            {
            if (!this.IsExtant)
                return 0;

            int r = si.Energy; // LDA &0C0E
            r >>= 2; // LSR A : LSR A
            r -= si.Energy; // SEC : SBC &0C0E
            r ^= 0xFF; // EOR #&FF
            r++; // CLC : ADC #01
            r &= 0xFF;

            int result = si.Energy - r;
            return result;
            }
        }
    }
