using System;

namespace Labyrinth.Services.Sound
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
