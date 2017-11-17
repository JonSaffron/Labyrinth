using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameTimer : GameComponent
        {
        public TimeSpan TimeRemaining { get; private set; }

        private readonly EventHandler _callBack;

        public static GameTimer AddGameTimer(TimeSpan timeToElapse, EventHandler callBack, bool isEnabled = true)
            {
            var game = GlobalServices.Game;
            var gt = new GameTimer(game, timeToElapse, callBack, isEnabled);
            game.Components.Add(gt);
            return gt;
            }

        private GameTimer(Game game, TimeSpan timeToElapse, EventHandler callBack, bool isEnabled = true) : base(game)
            {
            if (timeToElapse.Ticks < 0)
                throw new ArgumentOutOfRangeException("timeToElapse");
            if (callBack == null)
                throw new ArgumentNullException("callBack");

            this.TimeRemaining = timeToElapse;
            this._callBack = callBack;
            this.Enabled = isEnabled;
            this.UpdateOrder = 0;
            }

        public override void Update(GameTime gameTime)
            {
            if (!this.Enabled)
                return;

            this.TimeRemaining -= gameTime.ElapsedGameTime;
            if (TimeRemaining.Ticks <= 0)
                {
                this._callBack(this, new EventArgs());
                this.Dispose();
                }
            }
        }
    }
