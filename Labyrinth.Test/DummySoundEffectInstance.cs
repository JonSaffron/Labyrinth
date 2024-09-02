using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Test
    {
    class DummySoundEffectInstance : ISoundEffectInstance
        {
        private SoundState _state = SoundState.Stopped;
        private float _pan;
        private float _volume;
        private bool _restart;

        public DummySoundEffectInstance()
            {
            this.Volume = 1.0f;
            this.InstanceName = "Instance";
            }

        public void Dispose()
            {
            // nothing to do
            }

        public void Play()
            {
            this._state = SoundState.Playing;
            this._restart = false;
            }

        public void Stop()
            {
            this._state = SoundState.Stopped;
            this._restart = false;
            }

        public void Restart()
            {
            this._state = SoundState.Stopped;
            this._restart = true;
            }

        public string InstanceName { get; set; }

        public bool IsSetToRestart => this._restart;

        public SoundState State
            {
            get
                {
                return _state;
                }
            }

        public float Pan
            {
            get
                {
                return _pan;
                }
            set
                {
                if (value < -1.0f || value > 1.0f)
                    throw new ArgumentOutOfRangeException();
                _pan = value;
                }
            }

        public float Volume
            {
            get
                {
                return _volume;
                }
            set
                {
                if (value < 0.0f || value > 1.0f)
                    throw new ArgumentOutOfRangeException();
                _volume = value;
                }
            }
        }
    }