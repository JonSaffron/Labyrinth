using System;
using JetBrains.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Mine : MovingItem
        {
        [NotNull] private MineState _mineState;
        private TimeSpan _countdown;

        private readonly Animation _staticUnarmedAnimation;
        private readonly Animation _movingUnarmedAnimation;
        private readonly Animation _armedAnimation;

        public Mine(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            this.Energy = 240;
            this._staticUnarmedAnimation = Animation.StaticAnimation("Sprites/Shot/MineUnarmed");
            this._movingUnarmedAnimation = Animation.ManualAnimation("Sprites/Shot/MineUnarmed", 4);
            this._armedAnimation = Animation.LoopingAnimation("Sprites/Shot/MineArmed", 4);
            this.Ap.PlayAnimation(this._staticUnarmedAnimation);
            this._mineState = new InactiveState(this);
            }

        public override int DrawOrder => (int) SpriteDrawOrder.StaticItem;

        public void SteppedOnBy(MovingItem movingItem)
            {
            this._mineState.SteppedOnBy(movingItem);
            }

        public override bool Update(GameTime gameTime)
            {
            var result = this._mineState.Update(gameTime);
            return result;
            }

        public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
            {
            this._mineState.Draw(gt, spriteBatch);
            }

        public override bool IsExtant
            {
            get
                {
                var result = this._mineState.IsExtant();
                return result;
                }
            }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        public override ObjectSolidity Solidity => ObjectSolidity.Stationary;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Abstract class for the purposes of implementing the state pattern
        /// </summary>
        private abstract class MineState
            {
            protected readonly Mine Mine;

            protected MineState(Mine mine)
                {
                this.Mine = mine;
                }

            public abstract void SteppedOnBy(MovingItem movingItem);
            public abstract bool Update(GameTime gameTime);
            public abstract void Draw(GameTime gt, ISpriteBatch spriteBatch);
            public abstract bool IsExtant();
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The inactive state where the mine is waiting for the player to step away from it
        /// </summary>
        private class InactiveState : MineState
            {
            private static readonly TimeSpan TimeInState = TimeSpan.FromMilliseconds(1000);

            public InactiveState(Mine mine) : base(mine)
                {
                this.Mine._countdown = TimeSpan.Zero;
                }

            public override void SteppedOnBy(MovingItem movingItem)
                {
                if (movingItem is Player)
                    this.Mine._countdown = TimeSpan.Zero;
                }

            public override bool Update(GameTime gameTime)
                {
                this.Mine._countdown += gameTime.ElapsedGameTime;
                if (this.Mine._countdown < TimeInState) 
                    return true;
                this.Mine._countdown -= TimeInState;
                this.Mine._mineState = new CookingState(this.Mine);
                return true;
                }

            public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
                {
                this.Mine.Ap.PlayAnimation(this.Mine._staticUnarmedAnimation);
                this.Mine.Ap.Draw(gt, spriteBatch, this.Mine.Position);
                }

            public override bool IsExtant()
                {
                return true;
                }
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The cooking state where the mine is arming itself
        /// </summary>
        private class CookingState : MineState
            {
            public CookingState(Mine mine) : base(mine)
                {
                }

            public override void SteppedOnBy(MovingItem movingItem)
                {
                if (movingItem is Player)
                    this.Mine._mineState = new InactiveState(this.Mine);
                }

            public override bool Update(GameTime gameTime)
                {
                if (this.Mine.Ap.AdvanceManualAnimation(gameTime))
                    return true;
                this.Mine._mineState = new PrimedState(this.Mine);
                return true;
                }

            public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
                {
                this.Mine.Ap.PlayAnimation(this.Mine._movingUnarmedAnimation);
                this.Mine.Ap.Draw(gt, spriteBatch, this.Mine.Position);
                }

            public override bool IsExtant()
                {
                return true;
                }
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The primed state where the mine will be fired should anything touch it
        /// </summary>
        private class PrimedState : MineState
            {
            public PrimedState(Mine mine) : base(mine)
                {
                }

            public override void SteppedOnBy(MovingItem movingItem)
                {
                this.Mine._mineState = new FiredState(this.Mine);
                }

            public override bool Update(GameTime gameTime)
                {
                return true;
                }

            public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
                {
                this.Mine.Ap.PlayAnimation(this.Mine._armedAnimation);
                this.Mine.Ap.Draw(gt, spriteBatch, this.Mine.Position);
                }

            public override bool IsExtant()
                {
                return true;
                }
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The fired state where everything around will be consumed by explosions
        /// </summary>
        private class FiredState : MineState
            {
            private int _spreadPosition;
            private readonly int[] _energyEffects = { 240, 160, 80 };
            private static readonly TimeSpan TimeInEachSpread = TimeSpan.FromMilliseconds(100);

            public FiredState(Mine mine) : base(mine)
                {
                this.Mine.PlaySound(GameSound.MonsterDies);
                }

            public override void SteppedOnBy(MovingItem movingItem)
                {
                // nothing to do
                }

            public override bool Update(GameTime gameTime)
                {
                this.Mine._countdown += gameTime.ElapsedGameTime;
                if (this._spreadPosition == 0)
                    {
                    GlobalServices.GameState.AddExplosion(this.Mine.TilePosition.ToPosition(), this._energyEffects[0]);
                    this._spreadPosition++;
                    return false;
                    }

                if (this.Mine._countdown < TimeInEachSpread) 
                    return false;
                this.Mine._countdown -= TimeInEachSpread;
                TilePos tp = this.Mine.TilePosition;
                var positions = new TilePos[4];
                for (int i = 1; i <= this._spreadPosition; i++)
                    {
                    // Top right quadrant
                    positions[0] = new TilePos(tp.X + i, tp.Y + this._spreadPosition - i);

                    // Bottom right quadrant
                    positions[1] = new TilePos(tp.X + this._spreadPosition - i, tp.Y - i);

                    // Bottom left quadrant
                    positions[2] = new TilePos(tp.X - i, tp.Y - this._spreadPosition + i);

                    // Top left quadrant
                    positions[3] = new TilePos(tp.X - this._spreadPosition + i, tp.Y + i);
                    
                    int energy = this._energyEffects[this._spreadPosition];
                    for (int j = 0; j < 4; j++)
                        GlobalServices.GameState.AddExplosion(positions[j].ToPosition(), energy);
                    }
                this._spreadPosition++;
                return false;
                }

            public override void Draw(GameTime gt, ISpriteBatch spriteBatch)
                {
                // don't draw anything
                }

            public override bool IsExtant()
                {
                var result = this._spreadPosition < this._energyEffects.GetLength(0);
                return result;
                }
            }
        }
    }
