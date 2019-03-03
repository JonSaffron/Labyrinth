using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class TileReservation : StaticItem
        {
        public TileReservation([NotNull] AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Intangible);
            }

        public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
            {
            // nothing to draw
            }

        public override ObjectSolidity Solidity => ObjectSolidity.Stationary;

        public override bool IsExtant => true;

        public override int DrawOrder => 0;
        }
    }
