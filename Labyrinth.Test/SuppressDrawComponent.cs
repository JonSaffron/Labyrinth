using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class SuppressDrawComponent : GameComponent
        {
        public SuppressDrawComponent(Game game) : base(game)
            {
            }

        public override void Update(GameTime gameTime)
            {
            System.Diagnostics.Trace.WriteLine(gameTime.TotalGameTime.ToString());

            this.Game.SuppressDraw();
            }
        }
    }
