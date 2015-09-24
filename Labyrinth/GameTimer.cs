using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameTimer : GameComponent
        {
        public double TimeRemaining { get; private set; }
        private readonly GameTimerCompleted _callBack;

        public delegate void GameTimerCompleted(object sender, EventArgs args);

        public GameTimer(Game1 game, double timeToElapse, GameTimerCompleted callBack, bool isEnabled = true) : base(game)
            {
            if (timeToElapse < 0)
                throw new ArgumentOutOfRangeException("timeToElapse");
            if (callBack == null)
                throw new ArgumentNullException("callBack");

            this.TimeRemaining = timeToElapse;
            this._callBack = callBack;
            this.Enabled = isEnabled;
            this.Game.Components.Add(this);
            }

        public override void Update(GameTime gameTime)
            {
            this.TimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeRemaining <= 0)
                {
                this.Game.Components.Remove(this);
                this._callBack(this, new EventArgs());
                }
            }
        }
    }
