using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Test
    {
    class DummySoundEffectInstance : ISoundEffectInstance
        {
        private SoundState _state = SoundState.Stopped;
        private float _pan;
        private float _volume;
        private string _instanceName = "Instance";

        public DummySoundEffectInstance()
            {
            Volume = 1.0f;
            }

        public void Dispose()
            {
            // nothing to do
            }

        public void Play()
            {
            this._state = SoundState.Playing;
            }

        public void Stop()
            {
            this._state = SoundState.Stopped;
            }

        public string InstanceName
            {
            get
                {
                return this._instanceName;
                }
            set
                {
                this._instanceName = value;
                }
            }

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