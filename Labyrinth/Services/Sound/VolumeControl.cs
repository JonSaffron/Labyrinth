using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class VolumeControl : IVolumeControl
        {
        private const int TopVolume = 11;
        private int _currentVolume = 8;
        private const string SerialiseFileName = "volume.dat";
        private const string Muted = nameof(Muted);

        public static VolumeControl Instance { get; } = new VolumeControl();

        private VolumeControl()
            {
            // only one instance allowed
            }

        public void LoadState()
            {
            // format looks like "8 muted"
            string line = PersistentStorage.ReadSettings(SerialiseFileName) ?? string.Empty;

            string[] parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var queue = new Queue<string>(parts);
            if (queue.TryDequeue(out var volumeString))
                {
                if (int.TryParse(volumeString, out var volume) && volume >= 0 && volume <= TopVolume)
                    {
                    this._currentVolume = volume;
                    }
                }

            if (queue.TryDequeue(out var isMuted) && string.Equals(isMuted, Muted, StringComparison.OrdinalIgnoreCase))
                {
                Mute();
                }
            else
                {
                Unmute();
                }
            }

        public void SaveState()
            {
            string line = $"{this._currentVolume}{(this.IsMuted ? " " + Muted : string.Empty)}";
            PersistentStorage.WriteSettings(SerialiseFileName, line);
            }

        public void Mute()
            {
            SetMasterVolume(0);
            }

        public void Unmute()
            {
            SetMasterVolume(this._currentVolume);
            }

        public bool IsMuted
            {
            get
                {
                var result = (SoundEffect.MasterVolume == 0f);
                return result;
                }
            }

        public void TurnUpTheVolume()
            {
            this._currentVolume = IsMuted ? 1 : Math.Min(this._currentVolume + 1, TopVolume);
            SetMasterVolume(this._currentVolume);
            }

        public void TurnDownTheVolume()
            {
            if (IsMuted)
                return;
            this._currentVolume = Math.Max(0, this._currentVolume - 1);
            SetMasterVolume(this._currentVolume);
            }

        private static void SetMasterVolume(int volumeLevel)
            {
            if (volumeLevel < 0 || volumeLevel > TopVolume)
                throw new ArgumentOutOfRangeException(nameof(volumeLevel));
            float volume = 1.0f / TopVolume * volumeLevel;
            SoundEffect.MasterVolume = volume;
            }

        public override string ToString()
            {
            var result = "Volume is " + (IsMuted ? "muted" : this._currentVolume + "/" + TopVolume);
            return result;
            }
        }
    }
