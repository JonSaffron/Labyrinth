﻿using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Bang : StaticItem
        {
        private bool _isExtant;
        
        public Bang(World world, Vector2 position, BangType bangType) : base(world, position)
            {
            Animation a;
            switch (bangType)
                {
                case BangType.Short:
                    a = Animation.SingleAnimation(World, "Sprites/Props/ShortBang", 3);
                    break;
                case BangType.Long:
                    a = Animation.SingleAnimation(World, "Sprites/Props/LongBang", 3);
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
        }
    }
