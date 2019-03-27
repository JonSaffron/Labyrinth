using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class CrumblyWall : StaticItem
        {
        private readonly decimal _initialEnergy;
        private readonly StaticAnimation _animationPlayer;

        public CrumblyWall(Vector2 position, string textureName, int energy) : base(position)
            {
            this._animationPlayer = new StaticAnimation(this, textureName);
            this.Energy = energy;
            this._initialEnergy = energy;
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Wall);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Impassable);
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override void ReduceEnergy(int energyToRemove)
            {
            base.ReduceEnergy(energyToRemove);
            if (!this.IsExtant)
                {
                this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Stationary);
                }

            decimal percentageEnergyLeft = this.Energy / this._initialEnergy;
            this._animationPlayer.Position = 1m - percentageEnergyLeft;
            }
        }
    }
