﻿using System;

namespace Labyrinth.Services.Sound
    {
    /// <summary>
    /// Initiates the playing of a single sound
    /// </summary>
    public class SoundPlayer : ISoundPlayer
        {
        public SoundLibrary SoundLibrary { get; }
        public IActiveSoundService ActiveSoundService { get; }

        public SoundPlayer(SoundLibrary soundLibrary, IActiveSoundService activeSoundService)
            {
            this.SoundLibrary = soundLibrary;
            this.ActiveSoundService = activeSoundService;
            }

        public void Play(GameSound gameSound)
            {
            InternalPlay(gameSound);
            }

        public void PlayWithCallback(GameSound gameSound, EventHandler callback)
            {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            InternalPlay(gameSound);
            AddCallback(gameSound, callback);
            }

        public void PlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (centrePointProvider == null)
                throw new ArgumentNullException(nameof(centrePointProvider));

            InternalPlayForObject(gameSound, gameObject, centrePointProvider);
            }

        public void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider, EventHandler callback)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (centrePointProvider == null)
                throw new ArgumentNullException(nameof(centrePointProvider));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            InternalPlayForObject(gameSound, gameObject, centrePointProvider);
            AddCallback(gameSound, callback);
            }

        private void InternalPlay(GameSound gameSound)
            {
#if DEBUG
            if (DoesSoundRequirePosition(gameSound))
                throw new ArgumentOutOfRangeException($"GameSound {gameSound} needs to be associated with a GameObject.");
#endif

            ISoundEffectInstance soundEffect = this.SoundLibrary[gameSound];
            var activeSound = new ActiveSound(soundEffect);
            this.ActiveSoundService.Add(activeSound);
            }

        private void InternalPlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider)
            {
#if DEBUG
            if (!DoesSoundRequirePosition(gameSound))
                throw new ArgumentOutOfRangeException($"GameSound {gameSound} should not be associated with a GameObject.");
#endif

            ISoundEffectInstance soundEffect = this.SoundLibrary[gameSound];
            var activeSound = new ActiveSoundForObject(soundEffect, gameObject, centrePointProvider);
            this.ActiveSoundService.Add(activeSound);
            }

        private void AddCallback(GameSound gameSound, EventHandler callback)
            {
            var duration = this.SoundLibrary.GetDuration(gameSound);
            GameTimer.AddGameTimer(duration, callback);
            }

        private static bool DoesSoundRequirePosition(GameSound gameSound)
            {
            var si = gameSound.GetAttributeOfType<SoundInfoAttribute>() ?? new SoundInfoAttribute();
            bool result = si.RequiresGameObject;
            return result;
            }
        }
    }
