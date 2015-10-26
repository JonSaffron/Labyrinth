﻿using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Grave : StaticItem
        {
        public Grave(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            this.Energy = 255;
            
            var a = Animation.StaticAnimation("Sprites/Props/Grave");
            this.Ap.PlayAnimation(a);
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.StaticItem;
                }
            }
        }
    }
