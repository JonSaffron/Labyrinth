using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Bang : MovingItem
        {
        private bool _isExtant;
        
        public Bang(AnimationPlayer animationPlayer, Vector2 position, BangType bangType) : base(animationPlayer, position)
            {
            Animation a;
            switch (bangType)
                {
                case BangType.Short:
                    a = Animation.ManualAnimation("Sprites/Props/ShortBang", 3);
                    break;
                case BangType.Long:
                    a = Animation.ManualAnimation("Sprites/Props/LongBang", 3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bangType));
                }
            
            Ap.PlayAnimation(a);
            this._isExtant = true;
            }

        public override bool IsExtant => this._isExtant;

        public override int DrawOrder => (int) SpriteDrawOrder.Bang;
        public override bool Update(GameTime gameTime)
            {
            if (!this.Ap.AdvanceManualAnimation(gameTime))
                this._isExtant = false;
            return this._isExtant;
            }
        }
    }
