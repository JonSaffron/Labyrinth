using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    class GameSoundInstance : IGameSoundInstance
        {
        private readonly SoundEffectInstance _sourceEffectInstance;

        public GameSoundInstance(SoundEffectInstance soundEffectInstance)
            {
            this._sourceEffectInstance = soundEffectInstance;
            }

        public void Play()
            {
            this._sourceEffectInstance.Play();
            }

        public float Pitch
            {
            get
                {
                return this._sourceEffectInstance.Pitch;
                }
            set
                {
                this._sourceEffectInstance.Pitch = value;
                }
            }
        }
    }
