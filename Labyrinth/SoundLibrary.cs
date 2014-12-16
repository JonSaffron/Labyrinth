using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth
    {
    public sealed class SoundLibrary : IDisposable, ISoundLibrary
        {
        private readonly Dictionary<GameSound, SoundEffect> _sounds;
        private readonly Dictionary<SoundEffectInstance, Tuple<GameSound, SoundEffectFinished>> _trackingInstances = new Dictionary<SoundEffectInstance, Tuple<GameSound, SoundEffectFinished>>();

        public delegate void SoundEffectFinished(object sender, SoundEffectFinishedEventArgs args);
    
        public SoundLibrary()
            {
            this._sounds = new Dictionary<GameSound, SoundEffect>();
            }

        public void LoadContent(ContentManager cm)
            {
            foreach (GameSound item in Enum.GetValues(typeof(GameSound)))
                {
                string path = string.Format("Sounds/{0}", Enum.GetName(typeof(GameSound), item));
                var se = cm.Load<SoundEffect>(path);
                this._sounds.Add(item, se);
                }
            }

        /// <summary>
        /// Retrieves an instance of a sound effect.
        /// </summary>
        /// <param name="gameSound">Identifies the sound effect</param>
        /// <returns>A reference to a sound effect. This should be retained whilst the sound is playing.</returns>
        public IGameSoundInstance GetSoundEffectInstance(GameSound gameSound)
            {
            var soundEffectInstance = this._sounds[gameSound].CreateInstance();
            var result = new GameSoundInstance(soundEffectInstance);
            return result;
            }

        public void Play(GameSound gameSound)
            {
            SoundEffect soundEffect = this._sounds[gameSound];
            soundEffect.Play();
            }

        public void Play(GameSound gameSound, SoundEffectFinished callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            SoundEffectInstance soundEffectInstance = this._sounds[gameSound].CreateInstance();
            this._trackingInstances.Add(soundEffectInstance, new Tuple<GameSound, SoundEffectFinished>(gameSound, callback));
            soundEffectInstance.Play();
            }

        public void CheckForStoppedInstances()
            {
            lock (this._trackingInstances)
                {
                var instancesToDispose = new List<SoundEffectInstance>();
                foreach (var instance in this._trackingInstances.Where(i => i.Key.State == SoundState.Stopped))
                    {
                    var gameSound = instance.Value.Item1;
                    var callback = instance.Value.Item2;
                    var args = new SoundEffectFinishedEventArgs(gameSound);
                    callback(this, args);

                    instancesToDispose.Add(instance.Key);
                    }

                foreach (SoundEffectInstance instance in instancesToDispose)
                    {
                    this._trackingInstances.Remove(instance);
                    instance.Dispose();
                    }
                }
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
                lock (this._trackingInstances)
                    {
                    var instancesToDispose = new List<SoundEffectInstance>(this._trackingInstances.Keys);

                    foreach (SoundEffectInstance instance in instancesToDispose)
                        {
                        this._trackingInstances.Remove(instance);
                        instance.Dispose();
                        }
                    
                    instancesToDispose.Clear();
                    }                
                }

            this._disposed = true;
            }
#endregion
        }
    }
