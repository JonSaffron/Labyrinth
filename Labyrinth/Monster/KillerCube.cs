using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    abstract class KillerCube : Monster
        {
        protected KillerCube(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }
        }
    }
