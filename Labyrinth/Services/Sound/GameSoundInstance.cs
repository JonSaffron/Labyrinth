using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    class GameSoundInstance : IGameSoundInstance
        {
        private readonly SoundEffectInstance _sourceEffectInstance;

        public GameSoundInstance(SoundEffectInstance soundEffectInstance)
            {
            this._sourceEffectInstance = soundEffectInstance;
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

        public void Play(ISoundPlayer soundPlayer)
            {
            soundPlayer.Play(this._sourceEffectInstance);
            }
        }
    }
