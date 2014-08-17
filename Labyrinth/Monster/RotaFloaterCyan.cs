using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class RotaFloaterCyan : RotaFloater
        {
        public RotaFloaterCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/RotaFloaterCyan");
            this.NormalAnimation = Animation.LoopingAnimation(t, 2);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed * 1.5f;
            this.ChanceOfAggressiveMove = 3;
            this.Mobility = MonsterMobility.Placid;
            }
        }
    }
