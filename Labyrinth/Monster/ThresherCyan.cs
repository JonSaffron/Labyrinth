using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class ThresherCyan : Thresher
        {
        public ThresherCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/ThresherCyan", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.LaysMushrooms = true;
            }
        }
    }
