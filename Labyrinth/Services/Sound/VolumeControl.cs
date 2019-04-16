using System;
using System.IO;
using System.IO.IsolatedStorage;
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
            }

        public void LoadState()
            {
            using (var storage = IsolatedStorageFile.GetUserStoreForDomain())
                {
                if (storage.FileExists(SerialiseFileName))
                    {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(SerialiseFileName, FileMode.Open, FileAccess.Read))
                        {
                        using (TextReader reader = new StreamReader(stream))
                            {
                            // "8 muted"
                            string line = reader.ReadLine();
                            if (line != null)
                                {
                                string[] parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length > 0)
                                    {
                                    if (int.TryParse(parts[0], out var volume))
                                        {
                                        if (volume >= 0 && volume <= TopVolume)
                                            {
                                            this._currentVolume = volume;
                                            SetVolume();
                                            }
                                        }
                                    }

                                if (parts.Length > 1)
                                    {
                                    if (string.Equals(parts[1], Muted))
                                        {
                                        Mute();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        public void SaveState()
            {
            using (var storage = IsolatedStorageFile.GetUserStoreForDomain())
                {
                using (IsolatedStorageFileStream stream = storage.CreateFile(SerialiseFileName))
                    {
                    using (TextWriter writer = new StreamWriter(stream))
                        {
                        string line = $"{this._currentVolume}{(this.IsMuted ? " " + Muted : string.Empty)}";
                        writer.WriteLine(line);
                        }
                    }
                }
            }

        public void Mute()
            {
            SoundEffect.MasterVolume = 0;
            }

        public void Unmute()
            {
            SetVolume();
            }

        public bool IsMuted
            {
            get
                {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                var result = (SoundEffect.MasterVolume == 0);
                return result;
                }
            }

        public void TurnUpTheVolume()
            {
            this._currentVolume = IsMuted ? 1 : Math.Min(this._currentVolume + 1, TopVolume);
            SetVolume();
            }

        public void TurnDownTheVolume()
            {
            if (IsMuted)
                return;
            this._currentVolume = Math.Max(0, this._currentVolume - 1);
            SetVolume();
            }

        private void SetVolume()
            {
            float volume = 1.0f / TopVolume * this._currentVolume;
            SoundEffect.MasterVolume = volume;
            }

        public override string ToString()
            {
            var result = "Volume is " + (IsMuted ? "muted" : this._currentVolume + "/" + TopVolume);
            return result;
            }
        }
    }
