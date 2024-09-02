using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    /// <summary>
    /// An object for playing a one sound once which is not connected to a GameObject but is simply an ambience
    /// </summary>
    /// <remarks>Multiple ActiveSound and ActiveSoundForObject instances can reference the same SoundEffectInstance</remarks>
    public class ActiveSound : IActiveSound
        {
        public ISoundEffectInstance SoundEffectInstance { get; }

        public ActiveSound(ISoundEffectInstance soundEffectInstance)
            {
            this.SoundEffectInstance = soundEffectInstance ?? throw new ArgumentNullException(nameof(soundEffectInstance));
            }

        public bool HasFinishedPlaying
            {
            get
                {
                var result = this.SoundEffectInstance is { State: SoundState.Stopped, IsSetToRestart: false };
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
                this.SoundEffectInstance.Restart();
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
            if (this.SoundEffectInstance is { IsSetToRestart: true, State: SoundState.Stopped })
                {
                InternalPlay();
                }
            }

        public override string ToString()
            {
            var result = $"{this.SoundEffectInstance.InstanceName} {this.SoundEffectInstance.State}";
            if (this.SoundEffectInstance.IsSetToRestart)
                result += " to be restarted";
            return result;
            }
        }
    }
