using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Bang : MovingItem
        {
        private bool _isExtant;
        private readonly AnimationPlayer _animationPlayer;

        public Bang(Vector2 position, BangType bangType) : base(position)
            {
            this._animationPlayer = new AnimationPlayer(this);
            Animation a;
            switch (bangType)
                {
                case BangType.Short:
                    a = Animation.LinearAnimation("Sprites/Props/ShortBang", 3);
                    break;
                case BangType.Long:
                    a = Animation.LinearAnimation("Sprites/Props/LongBang", 12);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bangType));
                }
            
            this._animationPlayer.PlayAnimation(a);
            this._isExtant = true;
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Bang);
            }

        public override bool IsExtant => this._isExtant;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        
        public override bool Update(GameTime gameTime)
            {
            this._animationPlayer.Update(gameTime);
            if (this._animationPlayer.Position == 1)
                this._isExtant = false;
            return false;
            }

        }
    }
