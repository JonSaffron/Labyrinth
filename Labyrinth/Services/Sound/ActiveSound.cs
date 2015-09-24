using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSound : IActiveSound
        {
        public ISoundEffectInstance SoundEffectInstance { get; private set; }
        protected bool RestartPlay;

        public ActiveSound(ISoundEffectInstance soundEffectInstance)
            {
            if (soundEffectInstance == null)
                throw new ArgumentNullException("soundEffectInstance");
            this.SoundEffectInstance = soundEffectInstance;
            }

        public bool CanBeRemoved
            {
            get
                {
                var result = this.SoundEffectInstance.State == SoundState.Stopped && !this.RestartPlay;
                return result;
                }
            }

        protected virtual void InternalPlay()
            {
            this.SoundEffectInstance.Volume = 1.0f;
            this.SoundEffectInstance.Pan = 0.0f;
            this.SoundEffectInstance.Play();
            }

        public virtual void Play()
            {
            if (this.SoundEffectInstance.State == SoundState.Playing)
                {
                this.SoundEffectInstance.Stop();
                this.RestartPlay = true;
                return;
                }

            InternalPlay();
            }

        public void Stop()
            {
            this.SoundEffectInstance.Stop();
            }

        public virtual void Update()
            {
            if (this.RestartPlay && this.SoundEffectInstance.State == SoundState.Stopped)
                {
                InternalPlay();
                this.RestartPlay = false;
                }
            }
        }
    }