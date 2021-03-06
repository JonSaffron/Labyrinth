﻿using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    class SoundEffectInstance : ISoundEffectInstance
        {
        private readonly Microsoft.Xna.Framework.Audio.SoundEffectInstance _soundEffectInstance;

        public SoundEffectInstance(Microsoft.Xna.Framework.Audio.SoundEffectInstance soundEffectInstance)
            {
            this._soundEffectInstance = soundEffectInstance;
            }

        public void Play()
            {
            this._soundEffectInstance.Play();
            }

        public void Stop()
            {
            this._soundEffectInstance.Stop();
            }

        public string InstanceName { get; set; }

        public bool RestartPlayWhenStopped { get; set; }

        public SoundState State => this._soundEffectInstance.State;

        public float Pan
            {
            get => this._soundEffectInstance.Pan;
            set => this._soundEffectInstance.Pan = value;
            }

        public float Volume
            {
            get => this._soundEffectInstance.Volume;
            set => this._soundEffectInstance.Volume = value;
            }

        public void Dispose()
            {
            if (!this._soundEffectInstance.IsDisposed)
                {
                this._soundEffectInstance.Dispose();
                }
            }

        public override string ToString()
            {
            var result = $"{this.InstanceName} {this.State} vol={this.Volume} pan={this.Pan}";
            return result;
            }
        }
    }
