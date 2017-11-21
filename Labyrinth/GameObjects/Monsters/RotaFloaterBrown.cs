using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class RotaFloaterBrown : RotaFloater
        {
        //private int _rotation;

        public RotaFloaterBrown(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/RotaFloaterBrown", 2));
            
            this.Mobility = MonsterMobility.Placid;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            }

        //public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
        //    {
        //    if (gt.ElapsedGameTime != TimeSpan.Zero && this._determineDirection is StandardRolling stdRolling)
        //        {
        //        switch (stdRolling.CurrentDirection)
        //            {
        //            case Direction.Right:
        //            case Direction.Down:
        //                this._rotation = (this._rotation + 1) % 100;
        //                break;
        //            case Direction.Up:
        //            case Direction.Left:
        //                this._rotation = (this._rotation + 99) % 100;
        //                break;
        //            }
        //        }
            
        //    this.Ap.Rotation = (float)(2 * Math.PI * this._rotation / 100f);
        //    base.Draw(gt, spriteBatch);
        //    }
        }
    }
