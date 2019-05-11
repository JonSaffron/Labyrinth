using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth.Services.Sound
    {
    public sealed class SoundLibrary : IDisposable
        {
        private Dictionary<string, SoundEffect> _soundResources;
        private Dictionary<GameSound,  SoundEffectInstanceCache> _cachedInstances;

        private const string SoundsFolder = "Sounds";

        public void LoadContent(ContentManager cm)
            {
            var listOfWavFiles = GetListOfWavFiles(cm.RootDirectory);
            this._soundResources = LoadResources(cm, listOfWavFiles);
            this._cachedInstances = BuildCache();
            this._disposed = false;
            }

        public ISoundEffectInstance this[GameSound gameSound]
            {
            get
                {
                if (this._disposed)
                    throw new ObjectDisposedException("SoundLibrary");
                if (this._cachedInstances == null)
                    throw new InvalidOperationException("Library resources have not been loaded - call LoadContent first.");

                var result = this._cachedInstances[gameSound].GetNext();
                return result;
                }
            }

        public TimeSpan GetDuration(GameSound gameSound)
            {
            if (this._disposed)
                throw new ObjectDisposedException("SoundLibrary");
            if (this._cachedInstances == null)
                throw new InvalidOperationException("Library resources have not been loaded - call LoadContent first.");

            var result = this._cachedInstances[gameSound].SoundDuration;
            return result;
            }

        private static IEnumerable<FileInfo> GetListOfWavFiles(string rootDirectoryForResources)
            {
            var pathToSoundsFolder = $@"{rootDirectoryForResources}\{SoundsFolder}";
            var dir = new DirectoryInfo(pathToSoundsFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException(pathToSoundsFolder);
            FileInfo[] files = dir.GetFiles("*.xnb");
            return files;
            } 

        private static Dictionary<string, SoundEffect> LoadResources(ContentManager cm, IEnumerable<FileInfo> listOfWavFiles)
            {
            var result = new Dictionary<string, SoundEffect>();
            foreach (var file in listOfWavFiles)
                {
                var resourceName = Path.GetFileNameWithoutExtension(file.Name);
                var pathToResource = $"{SoundsFolder}/{resourceName}";
                var soundEffect = cm.Load<SoundEffect>(pathToResource);
                soundEffect.Name = resourceName;
                result.Add(resourceName, soundEffect);
                }
            return result;
            }

        private Dictionary<GameSound,  SoundEffectInstanceCache> BuildCache()
            {
            var result = new Dictionary<GameSound,  SoundEffectInstanceCache>();
            foreach (GameSound item in Enum.GetValues(typeof(GameSound)))
                {
                var copyOfItem = item;
                ISoundEffectInstance CreateSoundEffectInstance() => CreateInstance(copyOfItem);

                int cacheSize = GetCacheSize(item);
                TimeSpan soundDuration = GetSoundEffect(copyOfItem).Duration;
                var cache = new SoundEffectInstanceCache(cacheSize, soundDuration, copyOfItem, CreateSoundEffectInstance);
                result.Add(item, cache);
                }
            return result;
            }

        private ISoundEffectInstance CreateInstance(GameSound gameSound)
            {
            var sei = GetSoundEffect(gameSound).CreateInstance();
            switch (gameSound)
                {
                case GameSound.PlayerMovesSecondFoot:
                    sei.Pitch = -0.15f;
                    break;
                }
            var result = new SoundEffectInstance(sei);
            return result;
            }

        private SoundEffect GetSoundEffect(GameSound gameSound)
            {
            var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
            string resourceName = si.ResourceName ?? gameSound.ToString();
            var result = this._soundResources[resourceName];
            return result;
            }

        private static int GetCacheSize(GameSound gameSound)
            {
            var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
            int result = si.CacheSize;
            return result;
            }

#region Disposable

        private bool _disposed;

        public void Dispose()
            {
            if (this._cachedInstances != null)
                {
                foreach (var item in this._cachedInstances)
                    {
                    item.Value.Dispose();
                    }
                this._cachedInstances = null;
                }
            if (this._soundResources != null)
                {
                foreach (var item in this._soundResources)
                    {
                    item.Value.Dispose();
                    }
                this._soundResources = null;
                }

            this._disposed = true;
            }
#endregion
        }
    }
