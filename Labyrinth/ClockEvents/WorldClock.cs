using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Labyrinth.ClockEvents
    {
    internal class WorldClock
        {
        private int _worldClock;
        private double _time;
        private readonly List<IClockEvent> _clockEvents = new List<IClockEvent>();

        public void Update(GameTime gameTime)
            {
            this._time += gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time >= Constants.GameClockResolution)
                {
                this._time -= Constants.GameClockResolution;
                if (this._worldClock < int.MaxValue)
                    {
                    Tick();
                    }
                }
            }

        private void Tick()
            {
            this._worldClock++;
            this._clockEvents.ForEach(item => item.Update(this._worldClock));
            }

        public void AddEventHandler(IClockEvent clockEvent)
            {
            if (clockEvent == null) throw new ArgumentNullException(nameof(clockEvent));
            this._clockEvents.Add(clockEvent);
            }
        }
    }
