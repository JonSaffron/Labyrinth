using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            }

        public ISoundEffectInstance this[GameSound gameSound]
            {
            get
                {
                var result = this._cachedInstances[gameSound].GetNext();
                return result;
                }
            }

        public TimeSpan GetDuration(GameSound gameSound)
            {
            var result = this._cachedInstances[gameSound].SoundDuration;
            return result;
            }

        private static IEnumerable<FileInfo> GetListOfWavFiles(string rootDirectoryForResources)
            {
            var pathToSoundsFolder = string.Format(@"{0}\{1}", rootDirectoryForResources, SoundsFolder);
            var dir = new DirectoryInfo(pathToSoundsFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException(pathToSoundsFolder);
            FileInfo[] files = dir.GetFiles("*.wav");
            return files;
            } 

        private static Dictionary<string, SoundEffect> LoadResources(ContentManager cm, IEnumerable<FileInfo> listOfWavFiles)
            {
            var result = 
                (from file in listOfWavFiles 
                select Path.GetFileNameWithoutExtension(file.Name) into resourceName 
                select string.Format("{0}/{1}", SoundsFolder, resourceName) into pathToResource 
                select cm.Load<SoundEffect>(pathToResource)).ToDictionary(se => se.Name);
            return result;
            }

        private Dictionary<GameSound,  SoundEffectInstanceCache> BuildCache()
            {
            var result = new Dictionary<GameSound,  SoundEffectInstanceCache>();
            foreach (GameSound item in Enum.GetValues(typeof(GameSound)))
                {
                var copyOfItem = item;
                Func<ISoundEffectInstance> createSoundEffectInstance = () => CreateInstance(copyOfItem);

                int cacheSize = GetCacheSize(item);
                TimeSpan soundDuration = GetSoundEffect(copyOfItem).Duration;
                var cache = new SoundEffectInstanceCache(cacheSize, soundDuration, copyOfItem, createSoundEffectInstance);
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
            SoundEffect result;
            switch (gameSound)
                {
                case GameSound.PlayerMovesFirstFoot:
                case GameSound.PlayerMovesSecondFoot:
                    result = this._soundResources["PlayerMoves"];
                    break;
                default:
                    result = this._soundResources[gameSound.ToString()];
                    break;
                }
            return result;
            }

        private static int GetCacheSize(GameSound gameSound)
            {
            int result;
            switch (gameSound)
                {
                case GameSound.PlayerCollectsCrystal:
                case GameSound.PlayerFinishesWorld:
                case GameSound.PlayerEntersNewLevel:
                case GameSound.PlayerDies:
                case GameSound.PlayerMovesFirstFoot:
                case GameSound.PlayerMovesSecondFoot:
                case GameSound.PlayerStartsNewLife:
                    result = 1;
                    break;

                default:
                    result = 3;
                    break;
                }
            return result;
            }

        private bool DoesSoundRequirePosition(GameSound gameSound)
            {
            switch (gameSound)
                {
                case GameSound.BoulderBounces:
                case GameSound.PlayerCollidesWithMonster:
                case GameSound.PlayerShootsAndInjuresEgg:
                case GameSound.MonsterDies:
                case GameSound.MonsterEntersRoom:
                case GameSound.EggHatches:
                case GameSound.PlayerShootsAndInjuresMonster:
                case GameSound.MonsterLaysEgg:
                case GameSound.MonsterLaysMushroom:
                case GameSound.MonsterLeavesRoom:
                case GameSound.MonsterShoots:
                case GameSound.ShotBounces:
                case GameSound.MonsterShattersIntoNewLife:
                case GameSound.StaticObjectShotAndInjured:
                    return true;
                }

            return false;
            }

#region Disposable

        private bool _disposed;

        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        ~SoundLibrary()
            {
            Dispose(false);
            }

        private void Dispose(bool isDisposing)
            {
            if (this._disposed)
                return;

            if (isDisposing)
                {
                if (this._cachedInstances != null)
                    {
                    foreach (var item in this._cachedInstances)
                        {
                        item.Value.Dispose();
                        }
                    }
                if (this._soundResources != null)
                    {
                    foreach (var item in this._soundResources)
                        {
                        item.Value.Dispose();
                        }
                    }
                }

            this._disposed = true;
            }
#endregion
        }
    }
