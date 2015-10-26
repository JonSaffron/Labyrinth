using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class RotaFloaterCyan : RotaFloater
        {
        public RotaFloaterCyan(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/RotaFloaterCyan", 2));
            
            this.ChanceOfAggressiveMove = 3;
            this.Mobility = MonsterMobility.Placid;
            }

        protected override decimal StandardSpeed
            {
            get
                {
                return Constants.BaseSpeed * 1.5m;
                }
            }
        }
    }
