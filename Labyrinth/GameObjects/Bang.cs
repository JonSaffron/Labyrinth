using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Bang : MovingItem
        {
        private bool _isExtant;
        private readonly LinearAnimation _animationPlayer;

        public Bang(Vector2 position, BangType bangType) : base(position)
            {
            switch (bangType)
                {
                case BangType.Short:
                    this._animationPlayer = new LinearAnimation(this, "Sprites/Props/ShortBang", 3);
                    break;
                case BangType.Long:
                    this._animationPlayer = new LinearAnimation(this, "Sprites/Props/LongBang", 12);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bangType));
                }
            
            this._isExtant = true;
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Bang);
            }

        public override bool IsExtant => this._isExtant;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        
        public override bool Update(GameTime gameTime)
            {
            if (!this._animationPlayer.HasReachedEndOfAnimation)
                {
                this._animationPlayer.Update(gameTime);
                }
            else
                {
                this._isExtant = false;
                }

            return false;
            }
        }
    }
