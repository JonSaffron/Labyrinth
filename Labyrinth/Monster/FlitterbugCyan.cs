using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugCyan : Flitterbug
        {
        public FlitterbugCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/FlitterbugCyan", 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Flitters = true;
            }
        }
    }
