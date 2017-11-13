using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class KillerCube : Monster
        {
        protected KillerCube(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.ShootBehaviour = MonsterShootBehaviour.Yes;
            }
        }
    }
