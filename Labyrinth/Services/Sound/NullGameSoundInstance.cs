﻿namespace Labyrinth.Services.Sound
    {
    class NullGameSoundInstance : IGameSoundInstance
        {
        public void Play(ISoundPlayer soundPlayer)
            {
            // nothing to do
            }

        public float Pitch { get; set; }
        }
    }