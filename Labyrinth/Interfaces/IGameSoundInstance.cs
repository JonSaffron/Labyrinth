namespace Labyrinth
    {
    public interface IGameSoundInstance
        {
        void Play(ISoundPlayer soundPlayer);
        float Pitch { get; set; }
        }
    }
