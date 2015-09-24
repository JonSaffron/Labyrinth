using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public interface ISoundPlayer
        {
        void Play(GameSound gameSound);
        void Play(GameSound gameSound, SoundEffectFinished callback);
        void Play(GameSound gameSound, StaticItem gameObject);
        void Play(GameSound gameSound, StaticItem gameObject, SoundEffectFinished callback);
        }
    }
