using Microsoft.Xna.Framework.Content;

namespace Labyrinth
    {
    public interface ISoundLibrary
        {
        void LoadContent(ContentManager cm);
        IGameSoundInstance GetSoundEffectInstance(GameSound gameSound);
        void Play(GameSound gameSound);
        void Play(GameSound gameSound, SoundLibrary.SoundEffectFinished callback);
        void CheckForStoppedInstances();
        }
    }
