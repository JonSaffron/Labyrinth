using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class RotaFloaterCyan : RotaFloater
        {
        public RotaFloaterCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/RotaFloaterCyan", 2));
            
            this.ChanceOfAggressiveMove = 3;
            this.Mobility = MonsterMobility.Placid;
            }

        protected override decimal StandardSpeed
            {
            get
                {
                return AnimationPlayer.BaseSpeed * 1.5m;
                }
            }
        }
    }
