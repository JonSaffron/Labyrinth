using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class FlitterbugCyan : Flitterbug
        {
        public FlitterbugCyan(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/FlitterbugCyan");
            this.NormalAnimation = Animation.LoopingAnimation(t, 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.Flitters = true;
            }
        }
    }
