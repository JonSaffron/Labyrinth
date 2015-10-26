using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class FlitterbugBrown : Flitterbug
        {
        public FlitterbugBrown(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/FlitterbugBrown", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.Flitters = true;
            this.SplitsOnHit = true;
            }
        }
    }
