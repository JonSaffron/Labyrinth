namespace Labyrinth
    {
    public interface IActiveSound
        {
        ISoundEffectInstance SoundEffectInstance { get; }
        bool CanBeRemoved { get; }

        void Play();
        void Stop();
        void Update();
        }
    }
