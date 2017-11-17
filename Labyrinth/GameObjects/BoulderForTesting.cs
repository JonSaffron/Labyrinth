using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class BoulderForTesting : Boulder
        {
        public BoulderForTesting(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            }

        public override ObjectCapability Capability => ObjectCapability.CanPushOthers;
        }
    }
