using System;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class VolumeControl : IVolumeControl
        {
        private const int TopVolume = 11;
        private int _currentVolume = 8;    // todo save and restore this value
        private const string SerialiseFileName = "volume.dat";
        public static VolumeControl Instance { get; } = new VolumeControl();

        public VolumeControl()
            {
            using (var storage = IsolatedStorageFile.GetUserStoreForDomain())
                {
                if (storage.FileExists(SerialiseFileName))
                    {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(SerialiseFileName, FileMode.Open, FileAccess.Read))
                        {
                        using (TextReader reader = new StreamReader(stream))
                            {
                            string line = reader.ReadLine();

                            }                        
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
