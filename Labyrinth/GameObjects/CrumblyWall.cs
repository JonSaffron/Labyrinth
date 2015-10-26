using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class CrumblyWall : StaticItem
        {
        private readonly int _initialEnergy;

        public CrumblyWall(AnimationPlayer animationPlayer, Vector2 position, string textureName, int energy) : base(animationPlayer, position)
            {
            var a = Animation.StaticAnimation(textureName);
            this.Ap.PlayAnimation(a);
            this.Energy = energy;
            this._initialEnergy = energy;
            }

        public override void ReduceEnergy(int energyToRemove)
            {
            base.ReduceEnergy(energyToRemove);

            int stages = this.Ap.FrameCount - 1;
            this.Ap.FrameIndex = stages - (int) Math.Floor(stages * this.Energy / (float) this._initialEnergy);
            }

        public override ObjectSolidity Solidity
            {
            get
                {
                var result = this.IsExtant ? ObjectSolidity.Impassable : ObjectSolidity.Stationary;
                return result;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Wall;
                }
            }
        }
    }
