using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class Mushroom : StaticItem
        {
        private bool _isTaken;

        public Mushroom(World world, Vector2 position) : base(world, position)
            {
            var texture = World.Content.Load<Texture2D>("Sprites/Props/Mushroom");
            var a = Animation.StaticAnimation(texture);
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

        public void SetTaken()
        {
            this._isTaken = true;
        }

        public int CalculateEnergyToRemove(Player player)
        {
            if (!this.IsExtant)
                return 0;

            int r = player.Energy; // LDA &0C0E
            r >>= 2; // LSR A : LSR A
            r -= player.Energy; // SEC : SBC &0C0E
            r ^= 0xFF; // EOR #&FF
            r++; // CLC : ADC #01
            r &= 0xFF;

            int result = player.Energy - r;
            return result;
            }
        }
    }
