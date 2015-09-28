using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSoundForObject : ActiveSound
        {
        public readonly IGameObject GameObject;
        public readonly ICentrePointProvider CentrePointProvider;

        public ActiveSoundForObject(ISoundEffectInstance soundEffectInstance, IGameObject gameObject, ICentrePointProvider centrePointProvider) : base(soundEffectInstance)
            {
            if (gameObject == null)
                throw new ArgumentNullException("gameObject");
            if (centrePointProvider == null)
                throw new ArgumentNullException("centrePointProvider");
            this.GameObject = gameObject;
            this.CentrePointProvider = centrePointProvider;
            }

        protected override void InternalPlay()
            {
            UpdateVolumeAndPanning();
            this.SoundEffectInstance.Play();
            }

        public override void Play()
            {
            if (this.GameObject.IsExtant)
                base.Play();
            }

        public override void Update()
            {
            if (!this.GameObject.IsExtant)
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
            var centrePoint = this.CentrePointProvider.CentrePoint;
            var differenceInPosition = GameObject.Position - centrePoint;

            var adjustedDistanceApart = (differenceInPosition * new Vector2(1, 1.6f)).Length();
            var relativeCloseness = (768.0f - adjustedDistanceApart) / 768.0f;
            var volume = Math.Max(0, relativeCloseness);
            this.SoundEffectInstance.Volume = volume; 

            var distanceToTheSide = Math.Abs(differenceInPosition.X);
            var panning = Math.Min(1.0f, distanceToTheSide / 320.0f) * Math.Sign(differenceInPosition.X);
            this.SoundEffectInstance.Pan = panning;
            }
        }
    }