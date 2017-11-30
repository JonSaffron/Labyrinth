using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class FlitterbugCyan : Flitterbug
        {
        public FlitterbugCyan(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/FlitterbugCyan", 4));
            
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
