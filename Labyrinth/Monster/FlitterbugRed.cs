using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugRed : Flitterbug
        {
        public FlitterbugRed(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/FlitterbugRed", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Flitters = true;
            }
        }
    }
