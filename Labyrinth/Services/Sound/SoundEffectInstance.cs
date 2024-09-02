using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    internal class SoundEffectInstance : ISoundEffectInstance
        {
        private readonly Microsoft.Xna.Framework.Audio.SoundEffectInstance _soundEffectInstance;
        private bool _restart;

        public SoundEffectInstance(Microsoft.Xna.Framework.Audio.SoundEffectInstance soundEffectInstance, string instanceName)
            {
            this._soundEffectInstance = soundEffectInstance;
            this.InstanceName = instanceName ?? throw new ArgumentNullException(nameof(instanceName));
            }

        /// <inheritdoc />
        public void Play()
            {
            this._soundEffectInstance.Play();
            this._restart = false;
            }

        /// <inheritdoc />
        public void Stop()
            {
            this._soundEffectInstance.Stop(immediate: true);
            this._restart = false;
            }

        /// <inheritdoc />
        public void Restart()
            {
            this._soundEffectInstance.Stop(immediate: true);
            this._restart = true;
            }

        /// <inheritdoc />
        public string InstanceName { get; }

        /// <inheritdoc />
        public bool IsSetToRestart => this._restart;

        /// <inheritdoc />
        public SoundState State => this._soundEffectInstance.State;

        /// <inheritdoc />
        public float Pan
            {
            get => this._soundEffectInstance.Pan;
            set
                {
                if (!this._restart)
                    this._soundEffectInstance.Pan = value;
                }
            }

        /// <inheritdoc />
        public float Volume
            {
            get => this._soundEffectInstance.Volume;
            set
                {
                if (!this._restart)
                    this._soundEffectInstance.Volume = value;
                }
            }

        /// <inheritdoc />
        public void Dispose()
            {
            if (!this._soundEffectInstance.IsDisposed)
                {
                this._soundEffectInstance.Dispose();
                }
            }

        /// <inheritdoc />
        public override string ToString()
            {
            var result = $"{this.InstanceName} {this.State} vol={this.Volume} pan={this.Pan}";
            return result;
            }
        }
    }
