using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class ThresherCyan : Thresher
        {
        public ThresherCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/ThresherCyan");
            this.NormalAnimation = Animation.LoopingAnimation(t, 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.LaysMushrooms = true;
            }
        }
    }
