using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class FlitterbugRed : Flitterbug
        {
        public FlitterbugRed(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/FlitterbugRed", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Flitters = true;
            }
        }
    }
