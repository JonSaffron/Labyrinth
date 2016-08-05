using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class RotaFloaterBrown : RotaFloater
        {
        public RotaFloaterBrown(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/RotaFloaterBrown", 2));
            
            this.Mobility = MonsterMobility.Placid;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
