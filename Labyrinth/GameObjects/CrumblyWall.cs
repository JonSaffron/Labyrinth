using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class CrumblyWall : StaticItem
        {
        private readonly int _initialEnergy;
        private readonly AnimationPlayer _animationPlayer;

        public CrumblyWall(Vector2 position, string textureName, int energy) : base(position)
            {
            this._animationPlayer = new AnimationPlayer(this);
            var a = Animation.StaticAnimation(textureName);
            this._animationPlayer.PlayAnimation(a);
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

            float percentageEnergyLeft = this.Energy / (float) this._initialEnergy;
            this._animationPlayer.Position = 1.0 - percentageEnergyLeft;
            }
        }
    }
