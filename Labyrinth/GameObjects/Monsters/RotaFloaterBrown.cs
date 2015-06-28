using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class RotaFloaterBrown : RotaFloater
        {
        public RotaFloaterBrown(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/RotaFloaterBrown", 2));
            
            this.ChanceOfAggressiveMove = 6;
            this.Mobility = MonsterMobility.Placid;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }
        }
    }
