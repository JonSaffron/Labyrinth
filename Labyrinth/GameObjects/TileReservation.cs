using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class TileReservation : StaticItem
        {
        public TileReservation([NotNull] AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            this.DrawOrder = 0;
            this.IsExtant = true;
            this.Solidity = ObjectSolidity.Stationary;
            }

        public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
            {
            // nothing to draw
            }

        public override ObjectSolidity Solidity { get; }

        public override bool IsExtant { get; }

        public override int DrawOrder { get; }
        }
    }
