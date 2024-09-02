using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth.Services.Sound
    {
    public sealed class SoundLibrary : IDisposable
        {
        private readonly Dictionary<GameSound, SoundResource> _resources = new Dictionary<GameSound, SoundResource>();
        public bool IsDisposed { get; private set; }

        public void LoadContent(ContentManager cm)
            {
            var soundLibraryBuilder = new SoundLibraryBuilder();
            var sounds = soundLibraryBuilder.BuildSoundResources(cm);
            foreach (var item in sounds)
                {
                this._resources.Add(item.GameSound, item);
                }
            }

        public ISoundEffectInstance this[GameSound gameSound]
            {
            get
                {
                if (this.IsDisposed)
                    throw new ObjectDisposedException(nameof(SoundLibrary));
                if (this._resources.Count == 0)
                    throw new InvalidOperationException("Library resources have not been loaded - call LoadContent first.");

                var result = this._resources[gameSound].InstanceCache.GetNext();
                return result;
                }
            }

        public TimeSpan GetDuration(GameSound gameSound)
            {
            if (this.IsDisposed)
                throw new ObjectDisposedException(nameof(SoundLibrary));
            if (this._resources.Count == 0)
                throw new InvalidOperationException("Library resources have not been loaded - call LoadContent first.");

            var result = this._resources[gameSound].Duration;
            return result;
            }

        public void Dispose()
            {
            foreach (var item in this._resources.Values)
                {
                item.Dispose();
                }
            this._resources.Clear();

            this.IsDisposed = true;
            }

        private class SoundLibraryBuilder
            {
            private const string SoundsFolder = "Sounds";

            [Pure] public IEnumerable<SoundResource> BuildSoundResources(ContentManager cm)
                {
                var listOfWavFiles = GetListOfWavFiles(cm.RootDirectory);
                Dictionary<string, SoundEffect> soundEffectsByFileName = LoadResources(cm, listOfWavFiles);
                
                foreach (GameSound item in Enum.GetValues(typeof(GameSound)))
                    {
                    var soundEffect = GetSoundEffect(item, soundEffectsByFileName);
                    var copyOfItem = item;
                    Microsoft.Xna.Framework.Audio.SoundEffectInstance CreateSoundEffectInstance() => CreateInstance(copyOfItem, soundEffect);

                    int cacheSize = GetCacheSize(item);
                    var cache = new SoundEffectInstanceCache(cacheSize, item, CreateSoundEffectInstance);

                    var result = new SoundResource(item, soundEffect, cache);
                    yield return result;
                    }
                }

            private static IEnumerable<string> GetListOfWavFiles(string rootDirectoryForResources)
                {
                var pathToSoundsFolder = $@"{rootDirectoryForResources}\{SoundsFolder}";
                var dir = new DirectoryInfo(pathToSoundsFolder);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException(pathToSoundsFolder);
                var files = dir.GetFiles("*.xnb").Select(fileInfo => fileInfo.Name);
                return files;
                } 

            private static Dictionary<string, SoundEffect> LoadResources(ContentManager cm, IEnumerable<string> listOfWavFiles)
                {
                var result = new Dictionary<string, SoundEffect>();
                foreach (var filename in listOfWavFiles)
                    {
                    var resourceName = Path.GetFileNameWithoutExtension(filename);
                    var pathToResource = $"{SoundsFolder}/{resourceName}";
                    var soundEffect = cm.Load<SoundEffect>(pathToResource);
                    soundEffect.Name = resourceName;
                    result.Add(resourceName, soundEffect);
                    }
                return result;
                }

            private Microsoft.Xna.Framework.Audio.SoundEffectInstance CreateInstance(GameSound gameSound, SoundEffect soundEffect)
                {
                var soundEffectInstance = soundEffect.CreateInstance();
                switch (gameSound)
                    {
                    case GameSound.PlayerMovesSecondFoot:
                        soundEffectInstance.Pitch = -0.15f;
                        break;
                    }
                return soundEffectInstance;
                }

            private static SoundEffect GetSoundEffect(GameSound gameSound, Dictionary<string, SoundEffect> soundEffectsByFileName)
                {
                var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
                string resourceName = si.ResourceName ?? gameSound.ToString();
                var result = soundEffectsByFileName[resourceName];
                return result;
                }

            private static int GetCacheSize(GameSound gameSound)
                {
                var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
                int result = si.CacheSize;
                return result;
                }
            }

        private sealed class SoundResource : IDisposable
            {
            public readonly GameSound GameSound;
            private readonly SoundEffect _soundEffect;
            public TimeSpan Duration => this._soundEffect.Duration;
            public readonly SoundEffectInstanceCache InstanceCache;
            public bool IsDisposed { get; private set; }

            public SoundResource(GameSound gameSound, SoundEffect soundEffect, SoundEffectInstanceCache instanceCache)
                {
                if (!Enum.IsDefined(typeof(GameSound), gameSound))
                    throw new InvalidEnumArgumentException(nameof(gameSound), (int)gameSound, typeof(GameSound));
                this.GameSound = gameSound;
                this._soundEffect = soundEffect ?? throw new ArgumentNullException(nameof(soundEffect));
                this.InstanceCache = instanceCache ?? throw new ArgumentNullException(nameof(instanceCache));
                }

            public void Dispose()
                {
                if (!this.IsDisposed)
                    {
                    this._soundEffect.Dispose();
                    this.InstanceCache.Dispose();
                    this.IsDisposed = true;
                    }
                }
            }
        }
    }
