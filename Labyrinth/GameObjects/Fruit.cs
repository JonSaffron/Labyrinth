using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Fruit : StaticItem
        {
        private readonly FruitType _fruitType;
        private bool _isTaken;

        public Fruit(AnimationPlayer animationPlayer, Vector2 position, FruitType fruitType, int energy) : base(animationPlayer, position)
            {
            this._fruitType = fruitType;
            this.Energy = energy;
            
            string textureName = string.Format("Sprites/Fruit/{0:G}", _fruitType);
            var a = Animation.StaticAnimation(textureName);
            this.Ap.PlayAnimation(a);
            }

        public FruitType FruitType => this._fruitType;

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
