using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class RotaFloaterCyan : RotaFloater
        {
        public RotaFloaterCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/RotaFloaterCyan", 2);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed * 1.5f;
            this.ChanceOfAggressiveMove = 3;
            this.Mobility = MonsterMobility.Placid;
            }
        }
    }
