using System;

namespace Labyrinth
    {
    public class SoundEffectFinishedEventArgs : EventArgs
        {
        public GameSound GameSound { get; private set; }

        public SoundEffectFinishedEventArgs(GameSound gameSound)
            {
            this.GameSound = gameSound;
            }
        }
    }
