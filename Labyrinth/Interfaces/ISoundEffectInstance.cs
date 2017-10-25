using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    public interface ISoundEffectInstance : IDisposable
        {
        void Play();
        void Stop();

        string InstanceName { get; set; }
        SoundState State { get; }
        bool RestartPlayWhenStopped { get; set; }
        float Pan { get; set; }
        float Volume { get; set; }
        }
    }
