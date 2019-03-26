using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class TileReservation : StaticItem
        {
        public TileReservation(Vector2 position) : base(position)
            {
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Intangible);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Stationary);
            }

        public override IRenderAnimation RenderAnimation => null;

        public override bool IsExtant => true;
        }
    }
