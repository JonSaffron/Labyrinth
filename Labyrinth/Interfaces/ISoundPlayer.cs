using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    public interface ISoundPlayer
        {
        void Play(GameSound gameSound);
        void Play(GameSound gameSound, SoundEffectFinished callback);
        void Play(SoundEffectInstance soundEffectInstance);
        }
    }
