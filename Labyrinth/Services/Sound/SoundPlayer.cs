using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    class SoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;
        private readonly Game1 _game;
        private readonly IActiveSoundService _activeSoundService = new ActiveSoundService();
        private float _masterVolumeLevel = 1;

        public SoundPlayer(SoundLibrary soundLibrary, Game1 game)
            {
            this._soundLibrary = soundLibrary;
            this._game = game;
            SoundEffect.MasterVolume = _masterVolumeLevel;
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

        public void PlayForObject(GameSound gameSound, IGameObject gameObject)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");

            InternalPlay(gameSound, gameObject);
            }

        public void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, EventHandler callback)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");
            if (callback == null)
                throw new ArgumentNullException("callback");

            InternalPlay(gameSound, gameObject);
            AddCallback(gameSound, callback);
            }

        public void Enable()
            {
            SoundEffect.MasterVolume = this._masterVolumeLevel;
            }

        public void Disable()
            {
            SoundEffect.MasterVolume = 0;
            }

        public void TurnItUp()
            {
            this._masterVolumeLevel = Math.Min(1.0f, SoundEffect.MasterVolume + 0.1f);
            SoundEffect.MasterVolume = this._masterVolumeLevel;
            }

        public void TurnItDown()
            {
            this._masterVolumeLevel = Math.Max(0.0f, SoundEffect.MasterVolume - 0.1f);
            SoundEffect.MasterVolume = this._masterVolumeLevel;
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

        private void InternalPlay(GameSound gameSound, IGameObject gameObject)
            {
#if DEBUG
            if (!DoesSoundRequirePosition(gameSound))
                throw new ArgumentOutOfRangeException("GameSound " + gameSound + " should not be associated with a GameObject.");
#endif

            ISoundEffectInstance soundEffect = this._soundLibrary[gameSound];
            var activeSound = new ActiveSoundForObject(soundEffect, gameObject, this._game);
            this._activeSoundService.Add(activeSound);
            }

        private void AddCallback(GameSound gameSound, EventHandler callback)
            {
            var duration = this._soundLibrary.GetDuration(gameSound);
            GameTimer.AddGameTimer(this._game, duration, callback);
            }

        private static bool DoesSoundRequirePosition(GameSound gameSound)
            {
            switch (gameSound)
                {
                case GameSound.PlayerCollectsCrystal:
                case GameSound.PlayerFinishesWorld:
                case GameSound.PlayerEntersNewLevel:
                case GameSound.PlayerDies:
                case GameSound.PlayerStartsNewLife:
                    return false;
                }

            return true;
            }
        }
    }
