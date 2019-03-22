using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.ClockEvents
    {
    class WorldClock
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
            foreach (var item in this._clockEvents)
                {
                item.Update(this._worldClock);
                }
            }

        public void AddEventHandler([NotNull] IClockEvent clockEvent)
            {
            if (clockEvent == null) throw new ArgumentNullException(nameof(clockEvent));
            this._clockEvents.Add(clockEvent);
            }
        }
    }
