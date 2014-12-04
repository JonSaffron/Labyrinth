using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugBrown : Flitterbug
        {
        public FlitterbugBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/FlitterbugBrown", 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.Flitters = true;
            this.SplitsOnHit = true;
            }
        }
    }
