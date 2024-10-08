﻿using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Fruit : StaticItem
        {
        public FruitType FruitType { get; }
        private bool _isTaken;
        private readonly StaticAnimation _animationPlayer;

        public Fruit(Vector2 position, FruitType fruitType, int energy) : base(position)
            {
            this.FruitType = fruitType;
            this.Energy = energy;
            string textureName = $"Sprites/Fruit/{this.FruitType:G}";
            this._animationPlayer = new StaticAnimation(this, textureName);
            this.Properties.Set(GameObjectProperties.DrawOrder, SpriteDrawOrder.StaticItem);
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
