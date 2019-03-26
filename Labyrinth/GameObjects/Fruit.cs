using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Fruit : StaticItem
        {
        public FruitType FruitType { get; }
        private bool _isTaken;
        private readonly AnimationPlayer _animationPlayer;

        public Fruit(Vector2 position, FruitType fruitType, int energy) : base(position)
            {
            this.FruitType = fruitType;
            this.Energy = energy;
            this._animationPlayer = new AnimationPlayer(this);

            string textureName = $"Sprites/Fruit/{this.FruitType:G}";
            var a = Animation.StaticAnimation(textureName);
            this._animationPlayer.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticItem);
            }

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public void SetTaken()
            {
            this._isTaken = true;
            }
        }
    }
