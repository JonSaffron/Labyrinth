using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class ThresherBrown : Thresher
        {
        public ThresherBrown(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/ThresherBrown", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
