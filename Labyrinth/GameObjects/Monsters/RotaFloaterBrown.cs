using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class RotaFloaterBrown : RotaFloater
        {
        public RotaFloaterBrown(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/RotaFloaterBrown", 2));
            
            this.ChanceOfAggressiveMove = 6;
            this.Mobility = MonsterMobility.Placid;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
