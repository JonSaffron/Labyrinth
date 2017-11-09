using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSoundForObject : ActiveSound
        {
        private readonly IGameObject _gameObject;
        private readonly ICentrePointProvider _centrePointProvider;

        public ActiveSoundForObject([NotNull] ISoundEffectInstance soundEffectInstance, [NotNull] IGameObject gameObject, [NotNull] ICentrePointProvider centrePointProvider) : base(soundEffectInstance)
            {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));
            if (!gameObject.IsExtant)
                throw new ArgumentException("GameObject is not extant.");
            this._gameObject = gameObject;
            this._centrePointProvider = centrePointProvider ?? throw new ArgumentNullException(nameof(centrePointProvider));
            }

        protected override void InternalPlay()
            {
            UpdateVolumeAndPanning();
            this.SoundEffectInstance.Play();
            }

        public override void Play()
            {
            if (this._gameObject.IsExtant)
                base.Play();
            }

        public override void Update()
            {
            if (!this._gameObject.IsExtant)
                {
                this.Stop();
                return;
                }

            if (this.SoundEffectInstance.State == SoundState.Playing)
                UpdateVolumeAndPanning();

            base.Update();
            }

        private void UpdateVolumeAndPanning()
            {
            var centrePoint = this._centrePointProvider.CentrePoint;
            var differenceInPosition = _gameObject.Position - centrePoint;

            var adjustedDistanceApart = (differenceInPosition * new Vector2(1, 1.6f)).Length();
            var relativeCloseness = (768.0f - adjustedDistanceApart) / 768.0f;
            var volume = Math.Max(0, relativeCloseness);
            this.SoundEffectInstance.Volume = volume; 

            var distanceToTheSide = Math.Abs(differenceInPosition.X);
            var panning = Math.Min(1.0f, distanceToTheSide / 320.0f) * Math.Sign(differenceInPosition.X);
            this.SoundEffectInstance.Pan = panning;
            }

        public override string ToString()
            {
            var result = string.Format("{0} {1} for {2} vol={3} pan={4}", this.SoundEffectInstance.InstanceName, this.SoundEffectInstance.State, this._gameObject.GetType().Name, this.SoundEffectInstance.Volume, this.SoundEffectInstance.Pan);
            if (this.SoundEffectInstance.RestartPlayWhenStopped)
                result += " to be restarted";
            return result;
            }
        }
    }
