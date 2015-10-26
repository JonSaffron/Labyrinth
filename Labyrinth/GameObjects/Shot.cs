using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class Shot : MovingItem
        {
        protected Shot(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            }
        }
    }
