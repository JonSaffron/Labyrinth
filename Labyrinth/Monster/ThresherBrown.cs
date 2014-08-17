using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class ThresherBrown : Thresher
        {
        public ThresherBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/ThresherBrown");
            this.NormalAnimation = Animation.LoopingAnimation(t, 3);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;              //120.0f;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
