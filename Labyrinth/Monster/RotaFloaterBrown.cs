using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class RotaFloaterBrown : RotaFloater
        {
        public RotaFloaterBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/RotaFloaterBrown");
            this.NormalAnimation = Animation.LoopingAnimation(t, 2);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.ChanceOfAggressiveMove = 6;
            this.Mobility = MonsterMobility.Placid;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
