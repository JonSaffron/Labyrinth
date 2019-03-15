using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameTimer : GameComponent
        {
        private TimeSpan _timeRemaining;

        private readonly EventHandler _callBack;

        public static GameTimer AddGameTimer(TimeSpan timeToElapse, EventHandler callBack, bool isEnabled = true)
            {
            var game = GlobalServices.Game;
            var gt = new GameTimer(game, timeToElapse, callBack, isEnabled);
            game.Components.Add(gt);
            return gt;
            }

        private GameTimer(Game game, TimeSpan timeToElapse, [NotNull] EventHandler callBack, bool isEnabled = true) : base(game)
            {
            if (timeToElapse.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(timeToElapse));
            this._timeRemaining = timeToElapse;

            this._callBack = callBack ?? throw new ArgumentNullException(nameof(callBack));

            this.Enabled = isEnabled;
            this.UpdateOrder = 0;
            }

        public override void Update(GameTime gameTime)
            {
            if (!this.Enabled)
                return;

            this._timeRemaining -= gameTime.ElapsedGameTime;
            if (this._timeRemaining.Ticks <= 0)
                {
                this._callBack(this, new EventArgs());
                this.Dispose();
                }
            }

        public override string ToString()
            {
            return $"GameTimer {(this.Enabled ? "Enabled" : "Disabled")} {(this._timeRemaining.Ticks <= 0 ? "Elapsed" : this._timeRemaining + " remaining")}";
            }
        }
    }
