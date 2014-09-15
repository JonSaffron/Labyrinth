using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
                    var shortBang = base.World.Content.Load<Texture2D>("Sprites/Props/ShortBang");
                    a = Animation.SingleAnimation(shortBang, 3);
                    break;
                case BangType.Long:
                    var longBang = base.World.Content.Load<Texture2D>("Sprites/Props/LongBang");
                    a = Animation.SingleAnimation(longBang, 3);
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
