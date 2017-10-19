using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class SoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;
        private readonly IActiveSoundService _activeSoundService;
        private const int TopVolume = 11;
        private int _currentVolume = 8;    // todo save and restore this value

        public SoundPlayer(SoundLibrary soundLibrary, IActiveSoundService activeSoundService)
            {
            this._soundLibrary = soundLibrary;
            this._activeSoundService = activeSoundService;
            }

        public void Play(GameSound gameSound)
            {
            InternalPlay(gameSound);
            }

        public void PlayWithCallback(GameSound gameSound, EventHandler callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            InternalPlay(gameSound);
            AddCallback(gameSound, callback);
            }

        public void PlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");
            if (centrePointProvider == null)
                throw new ArgumentNullException("centrePointProvider");

            InternalPlayForObject(gameSound, gameObject, centrePointProvider);
            }

        public void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider, EventHandler callback)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");
            if (centrePointProvider == null)
                throw new ArgumentNullException("centrePointProvider");
            if (callback == null)
                throw new ArgumentNullException("callback");

            InternalPlayForObject(gameSound, gameObject, centrePointProvider);
            AddCallback(gameSound, callback);
            }

        public void Unmute()
            {
            SetVolume();
            }

        public void Mute()
            {
            SoundEffect.MasterVolume = 0;
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

        public SoundLibrary SoundLibrary
            {
            get
                {
                return this._soundLibrary;
                }
            }

        public IActiveSoundService ActiveSoundService
            {
            get 
                {
                return this._activeSoundService;
                }
            }

        private void SetVolume()
            {
            float volume = 1.0f / TopVolume * this._currentVolume;
            SoundEffect.MasterVolume = volume;
            }

        private void InternalPlay(GameSound gameSound)
            {
#if DEBUG
            if (DoesSoundRequirePosition(gameSound))
                throw new ArgumentOutOfRangeException("GameSound " + gameSound + " needs to be associated with a GameObject.");
#endif

            ISoundEffectInstance soundEffect = this._soundLibrary[gameSound];
            var activeSound = new ActiveSound(soundEffect);
            this._activeSoundService.Add(activeSound);
            }

        private void InternalPlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider)
            {
#if DEBUG
            if (!DoesSoundRequirePosition(gameSound))
                throw new ArgumentOutOfRangeException("GameSound " + gameSound + " should not be associated with a GameObject.");
#endif

            ISoundEffectInstance soundEffect = this._soundLibrary[gameSound];
            var activeSound = new ActiveSoundForObject(soundEffect, gameObject, centrePointProvider);
            this._activeSoundService.Add(activeSound);
            }

        private void AddCallback(GameSound gameSound, EventHandler callback)
            {
            var duration = this._soundLibrary.GetDuration(gameSound);
            GameTimer.AddGameTimer(duration, callback);
            }

        private static bool DoesSoundRequirePosition(GameSound gameSound)
            {
            var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
            bool result = si.RequiresGameObject;
            return result;
            }

        public override string ToString()
            {
            var result = "Volume is " + (IsMuted ? "muted" : this._currentVolume + "/" + TopVolume);
            return result;
            }
        }
    }
