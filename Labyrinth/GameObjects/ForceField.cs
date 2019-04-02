using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class ForceField : StaticItem
        {
        private readonly int _crystalRequired;
        private readonly LoopedAnimation _animationPlayer;
        private ForceFieldState _forceFieldState;

        public ForceField(Vector2 position, int crystalRequired) : base(position)
            {
            this._forceFieldState = new ActiveState(this);
            this._crystalRequired = crystalRequired;
            this._animationPlayer = new LoopedAnimation(this, "Sprites/Props/ForceField", 6);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Reflection);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.ForceField);
            }

        public override bool IsExtant => this._forceFieldState.IsExtent;

        public bool IsActive => this._forceFieldState is ActiveState;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool Update(GameTime gameTime)
            {
            this._animationPlayer.Update(gameTime);
            this._forceFieldState.Update(gameTime);
            return false;
            }

        private abstract class ForceFieldState
            {
            protected readonly ForceField ForceField;

            protected ForceFieldState(ForceField forceField)
                {
                this.ForceField = forceField;
                }

            public abstract void Update(GameTime gameTime);

            public virtual bool IsExtent => true;
            }

        private class ActiveState : ForceFieldState
            {
            public ActiveState(ForceField forceField) : base(forceField)
                {
                // nothing to do
                }

            public override void Update(GameTime gameTime)
                {
                bool isNeutralised = GlobalServices.GameState.Player.HasPlayerGotCrystal(this.ForceField._crystalRequired);
                if (isNeutralised)
                    {
                    this.ForceField._forceFieldState = new NeutralisedState(this.ForceField);
                    }
                }
            }

        private class NeutralisedState : ForceFieldState
            {
            public NeutralisedState(ForceField forceField) : base(forceField)
                {
                this.ForceField.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Intangible);
                }

            public override void Update(GameTime gameTime)
                {
                Rectangle room1 = World.GetContainingRoom(this.ForceField.Position);
                bool isPlayerInSameRoom = room1.ContainsPosition(GlobalServices.GameState.Player.Position);
                if (isPlayerInSameRoom)
                    {
                    this.ForceField._forceFieldState = new FadingState(this.ForceField);
                    }
                }
            }

        private class FadingState : ForceFieldState
            {
            private const float FadeTime = 2f;
            private double _fadeRemaining;

            public FadingState(ForceField forceField) : base(forceField)
                {
                this._fadeRemaining = FadeTime;
                forceField.PlaySound(GameSound.ForceFieldShutsDown);
                }
                
            public override void Update(GameTime gameTime)
                {
                this._fadeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this._fadeRemaining > 0)
                    {
                    this.ForceField._animationPlayer.Opacity = (float) (this._fadeRemaining / FadeTime);
                    }
                else
                    {
                    this.ForceField._forceFieldState = new FinishedState(this.ForceField);
                    }
                }
            }

        private class FinishedState : ForceFieldState
            {
            public FinishedState(ForceField forceField) : base(forceField)
                {
                // nothing to do
                }

            public override void Update(GameTime gameTime)
                {
                // nothing to do
                }

            public override bool IsExtent => false;
            }
        }
    }
