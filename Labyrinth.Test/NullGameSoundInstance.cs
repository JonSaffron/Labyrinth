namespace Labyrinth.Test
    {
    class NullGameSoundInstance : IGameSoundInstance
        {
        public void Play()
            {
            // nothing to do
            }

        public float Pitch { get; set; }
        }
    }
