using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class ThresherBrown : Thresher
        {
        public ThresherBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/ThresherBrown", 3);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;              //120.0f;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
