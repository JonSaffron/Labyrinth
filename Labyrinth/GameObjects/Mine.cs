using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Mine : MovingItem
        {
        [NotNull] private MineState _mineState;
        private TimeSpan _countdown;

        public Mine(Vector2 position) : base(position)
            {
            this.Energy = 240;
            this._mineState = new InactiveState(this);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticItem);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Stationary);
            }

        public void SteppedOnBy(IMovingItem movingItem)
            {
            this._mineState.SteppedOnBy(movingItem);
            }

        public override bool Update(GameTime gameTime)
            {
            var result = this._mineState.Update(gameTime);
            return result;
            }

        public override bool IsExtant
            {
            get
                {
                var result = this._mineState.IsExtant();
                return result;
                }
            }

        public override IRenderAnimation RenderAnimation => this._mineState.RenderAnimation;

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

            public abstract void SteppedOnBy(IMovingItem movingItem);
            public abstract bool Update(GameTime gameTime);
            public abstract bool IsExtant();
            public abstract IRenderAnimation RenderAnimation { get; }
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The inactive state where the mine is waiting for the player to step away from it
        /// </summary>
        private class InactiveState : MineState
            {
            private static readonly TimeSpan TimeInState = TimeSpan.FromMilliseconds(1000);
            private readonly StaticAnimation _animationPlayer;

            public InactiveState(Mine mine) : base(mine)
                {
                this.Mine._countdown = TimeSpan.Zero;
                this._animationPlayer = new StaticAnimation(mine, "Sprites/Shot/MineUnarmed");
                }

            public override void SteppedOnBy(IMovingItem movingItem)
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

            public override bool IsExtant()
                {
                return true;
                }

            public override IRenderAnimation RenderAnimation => this._animationPlayer;
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The cooking state where the mine is arming itself
        /// </summary>
        private class CookingState : MineState
            {
            private readonly LinearAnimation _animationPlayer;

            public CookingState(Mine mine) : base(mine)
                {
                this._animationPlayer = new LinearAnimation(mine, "Sprites/Shot/MineUnarmed", 48);
                }

            public override void SteppedOnBy(IMovingItem movingItem)
                {
                if (movingItem is Player)
                    this.Mine._mineState = new InactiveState(this.Mine);
                }

            public override bool Update(GameTime gameTime)
                {
                if (!this._animationPlayer.HasReachedEndOfAnimation)
                    {
                    this._animationPlayer.Update(gameTime);
                    }
                else
                    {
                    this.Mine._mineState = new PrimedState(this.Mine);
                    }
                return true;
                }

            public override bool IsExtant()
                {
                return true;
                }

            public override IRenderAnimation RenderAnimation => this._animationPlayer;
            }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The primed state where the mine will be fired should anything touch it
        /// </summary>
        private class PrimedState : MineState
            {
            private readonly LoopedAnimation _animationPlayer;

            public PrimedState(Mine mine) : base(mine)
                {
                this._animationPlayer = new LoopedAnimation(mine, "Sprites/Shot/MineArmed", 48);
                mine.PlaySound(GameSound.MineArmed);
                }

            public override void SteppedOnBy(IMovingItem movingItem)
                {
                this.Mine._mineState = new FiredState(this.Mine);
                }

            public override bool Update(GameTime gameTime)
                {
                this._animationPlayer.Update(gameTime);
                return true;
                }

            public override bool IsExtant()
                {
                return true;
                }

            public override IRenderAnimation RenderAnimation => this._animationPlayer;
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

            public override void SteppedOnBy(IMovingItem movingItem)
                {
                // nothing to do
                }

            public override bool Update(GameTime gameTime)
                {
                this.Mine._countdown += gameTime.ElapsedGameTime;
                if (this._spreadPosition == 0)
                    {
                    GlobalServices.GameState.AddExplosion(this.Mine.TilePosition.ToPosition(), this._energyEffects[0], this.Mine);
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
                        GlobalServices.GameState.AddExplosion(positions[j].ToPosition(), energy, this.Mine);
                    }
                this._spreadPosition++;
                return false;
                }

            public override IRenderAnimation RenderAnimation => null;

            public override bool IsExtant()
                {
                var result = this._spreadPosition < this._energyEffects.GetLength(0);
                return result;
                }
            }
        }
    }
