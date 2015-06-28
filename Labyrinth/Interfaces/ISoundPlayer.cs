using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    public interface ISoundPlayer
        {
        IGameSoundInstance GetSoundEffectInstance(GameSound gameSound);
        void Play(SoundEffectInstance soundEffectInstance);

        void Play(GameSound gameSound);
        void Play(GameSound gameSound, SoundEffectFinished callback);
        }
    }
