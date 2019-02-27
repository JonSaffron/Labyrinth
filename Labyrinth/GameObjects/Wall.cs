﻿using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Wall : StaticItem
        {
        public Wall(AnimationPlayer animationPlayer, Vector2 position, string textureName) : base(animationPlayer, position)
            {
            var a = Animation.StaticAnimation(textureName);
            this.Ap.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Impervious);
            }

        public override bool IsExtant { get; } = true;

        public override ObjectSolidity Solidity { get; } = ObjectSolidity.Impassable;

        public override int DrawOrder { get; } = (int) SpriteDrawOrder.Wall;
        }
    }
