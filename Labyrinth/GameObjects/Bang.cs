﻿using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Bang : StaticItem
        {
        private bool _isExtant;
        
        public Bang(AnimationPlayer animationPlayer, Vector2 position, BangType bangType) : base(animationPlayer, position)
            {
            Animation a;
            switch (bangType)
                {
                case BangType.Short:
                    a = Animation.SingleRunAnimation("Sprites/Props/ShortBang", 3);
                    break;
                case BangType.Long:
                    a = Animation.SingleRunAnimation("Sprites/Props/LongBang", 3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("bangType");
                }
            
            Ap.PlayAnimation(a);
            Ap.AnimationFinished += AnimationFinished;

            this._isExtant = true;
            }

        private void AnimationFinished(object sender, EventArgs e)
            {
            this._isExtant = false;
            }

        public override bool IsExtant
            {
            get
                {
                return this._isExtant;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Bang;
                }
            }
        }
    }
