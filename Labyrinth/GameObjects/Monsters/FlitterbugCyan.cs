using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugCyan : Flitterbug
        {
        public FlitterbugCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/FlitterbugCyan", 4));
            
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Flitters = true;
            }
        }
    }
