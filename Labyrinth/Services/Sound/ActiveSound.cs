using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSound : IActiveSound, IEqualityComparer<IActiveSound>
        {
        public ISoundEffectInstance SoundEffectInstance { get; private set; }
        public bool RestartPlayWhenStopped { get; private set; }

        public ActiveSound(ISoundEffectInstance soundEffectInstance)
            {
            if (soundEffectInstance == null)
                throw new ArgumentNullException("soundEffectInstance");
            this.SoundEffectInstance = soundEffectInstance;
            }

        public bool HasFinishedPlaying
            {
            get
                {
                var result = this.SoundEffectInstance.State == SoundState.Stopped && !this.RestartPlayWhenStopped;
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
                this.RestartPlayWhenStopped = true;
                return;
                }

            InternalPlay();
            }

        public void Stop()
            {
            this.SoundEffectInstance.Stop();
            this.RestartPlayWhenStopped = false;
            }

        public virtual void Update()
            {
            if (this.RestartPlayWhenStopped && this.SoundEffectInstance.State == SoundState.Stopped)
                {
                InternalPlay();
                this.RestartPlayWhenStopped = false;
                }
            }

        public bool Equals(IActiveSound x, IActiveSound y)
            {
            var result = x.SoundEffectInstance.InstanceName == y.SoundEffectInstance.InstanceName;
            return result;
            }

        public int GetHashCode(IActiveSound activeSound)
            {
            var result = activeSound.SoundEffectInstance.InstanceName.GetHashCode();
            return result;
            }

        public override string ToString()
            {
            var result = string.Format("{0} {1}", this.SoundEffectInstance.InstanceName, this.SoundEffectInstance.State);
            if (this.RestartPlayWhenStopped)
                result += " to be restarted";
            return result;
            }
        }
    }
