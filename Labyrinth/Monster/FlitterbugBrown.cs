using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugBrown : Flitterbug
        {
        public FlitterbugBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/FlitterbugBrown");
            this.NormalAnimation = Animation.LoopingAnimation(t, 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.Flitters = true;
            this.SplitsOnHit = true;
            }
        }
    }
