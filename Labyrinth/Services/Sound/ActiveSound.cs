using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSound : IActiveSound
        {
        public ISoundEffectInstance SoundEffectInstance { get; }

        public ActiveSound([NotNull] ISoundEffectInstance soundEffectInstance)
            {
            this.SoundEffectInstance = soundEffectInstance ?? throw new ArgumentNullException(nameof(soundEffectInstance));
            }

        public bool HasFinishedPlaying
            {
            get
                {
                var result = this.SoundEffectInstance.State == SoundState.Stopped && !this.SoundEffectInstance.RestartPlayWhenStopped;
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
                this.SoundEffectInstance.RestartPlayWhenStopped = true;
                return;
                }

            InternalPlay();
            }

        public void Stop()
            {
            this.SoundEffectInstance.Stop();
            this.SoundEffectInstance.RestartPlayWhenStopped = false;
            }

        public virtual void Update()
            {
            if (this.SoundEffectInstance.RestartPlayWhenStopped && this.SoundEffectInstance.State == SoundState.Stopped)
                {
                InternalPlay();
                this.SoundEffectInstance.RestartPlayWhenStopped = false;
                }
            }

        public override string ToString()
            {
            var result = string.Format("{0} {1}", this.SoundEffectInstance.InstanceName, this.SoundEffectInstance.State);
            if (this.SoundEffectInstance.RestartPlayWhenStopped)
                result += " to be restarted";
            return result;
            }
        }
    }
