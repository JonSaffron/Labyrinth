using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Fruit : StaticItem
        {
        private readonly FruitType _fruitType;
        private bool _isTaken;

        public Fruit(World world, Vector2 position, FruitType fruitType) : base(world, position)
            {
            this._fruitType = fruitType;
            switch (fruitType)
                {
                case FruitType.Apple:
                    this.Energy = 6;
                    break;
                case FruitType.Watermelon:
                    this.Energy = 8;
                    break;
                case FruitType.Pineapple:
                    this.Energy = 10;
                    break;
                case FruitType.Strawberry:
                    this.Energy = 12;
                    break;
                case FruitType.Cherries:
                    this.Energy = 14;
                    break;
                case FruitType.Acorn:
                    this.Energy = 16;
                    break;
                default:
                    throw new InvalidOperationException();
                }
            
            string textureName = string.Format("Sprites/Fruit/{0:G}", _fruitType);
            var a = Animation.StaticAnimation(textureName);
            this.Ap.PlayAnimation(a);
            }

        public FruitType FruitType
            {
            get
                {
                return this._fruitType;
                }
            }

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.StaticItem;
                }
            }

        public void SetTaken()
            {
            this._isTaken = true;
            }
        }
    }
