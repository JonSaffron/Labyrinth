using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Fruit : StaticItem
        {
        public FruitType FruitType { get; }
        private bool _isTaken;

        public Fruit(AnimationPlayer animationPlayer, Vector2 position, FruitType fruitType, int energy) : base(animationPlayer, position)
            {
            this.FruitType = fruitType;
            this.Energy = energy;
            
            string textureName = $"Sprites/Fruit/{this.FruitType:G}";
            var a = Animation.StaticAnimation(textureName);
            this.Ap.PlayAnimation(a);
            }


        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public override int DrawOrder => (int) SpriteDrawOrder.StaticItem;

        public void SetTaken()
            {
            this._isTaken = true;
            }
        }
    }
