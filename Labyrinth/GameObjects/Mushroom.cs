using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Mushroom : StaticItem
        {
        private bool _isTaken;
        private readonly StaticAnimation _animationPlayer;

        public Mushroom(Vector2 position) : base(position)
            {
            this._animationPlayer = new StaticAnimation(this, "Sprites/Props/Mushroom");
            this.Energy = 40;
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticItem);
            }
        
        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public void SetTaken()
            {
            this._isTaken = true;
            }

        public int CalculateEnergyToRemove(Player p)
            {
            if (!this.IsExtant)
                return 0;

            var result = p.Energy >> 2;
            return result;
            }
        }
    }
