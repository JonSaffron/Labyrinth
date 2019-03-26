﻿using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Wall : StaticItem
        {
        private readonly AnimationPlayer _animationPlayer;

        public Wall(Vector2 position, string textureName) : base(position)
            {
            this._animationPlayer = new AnimationPlayer(this);
            var a = Animation.StaticAnimation(textureName);
            this._animationPlayer.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Impervious);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Wall);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Impassable);
            }

        public override bool IsExtant { get; } = true;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        }
    }
