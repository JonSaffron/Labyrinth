using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.Services.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    internal class HeadsUpDisplay : IHeadsUpDisplay
        {
        private decimal _displayedScore;
        private int _displayedEnergy;
        private Texture2D? _digits;
        private Texture2D? _life;
        private SpriteFont? _statusFont;
        private readonly Queue<string> _messages = new Queue<string>();
        private MessageState? _messageState;

        public void LoadContent(ContentManager contentManager)
            {
            this._digits = contentManager.Load<Texture2D>("Display/Digits");
            this._life = contentManager.Load<Texture2D>("Display/Life");
            this._statusFont = contentManager.Load<SpriteFont>("Display/StatusFont");

            Messenger.Default.Register<WorldStatus>(this, QueueStatusMessage);
            }

        private Texture2D Digits
            {
            get
                {
                if (this._digits == null)
                    throw new InvalidOperationException("Content for Digits property has not been loaded");
                return this._digits;
                }
            }

        private Texture2D Life
            {
            get
                {
                if (this._life == null)
                    throw new InvalidOperationException("Content for Life property has not been loaded");
                return this._life;
                }
            }

        private SpriteFont StatusFont
            {
            get
                {
                if (this._statusFont == null)
                    throw new InvalidOperationException("Content for StatusFont property has not been loaded");
                return this._statusFont;
                }
            }

        public void Reset()
            {
            this._displayedScore = 0;
            this._displayedEnergy = 0;
            }

        public void DrawStatus(ISpriteBatch spriteBatch, GameTime gameTime, bool isPlayerExtant, int playerEnergy, decimal score, int livesLeft)
            {
            if (this._messageState == null && this._messages.TryDequeue(out string? message))
                {
                this._messageState = new MessageFadeInState(this, message);
                }

            spriteBatch.Begin();

            UpdateDisplayValues(playerEnergy, score);

            DrawEnergyRect(spriteBatch, gameTime.IsRunningSlowly);
            if (isPlayerExtant)
                DrawRemainingEnergy(spriteBatch, this._displayedEnergy, playerEnergy);

            DrawScoreAndLivesRect(spriteBatch);
            DrawScore(spriteBatch, this._displayedScore);
            DrawLives(spriteBatch, livesLeft);

            this._messageState?.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            }

        private void UpdateDisplayValues(int playerEnergy, decimal score)
            {
            if (score != this._displayedScore)
                {
                double difference = (double) (score - this._displayedScore);
                this._displayedScore += (int) Math.Ceiling(Math.Sqrt(difference));
                }

            if (playerEnergy > this._displayedEnergy)
                {
                this._displayedEnergy++;
                }
            else if (playerEnergy < this._displayedEnergy)
                {
                this._displayedEnergy = playerEnergy;
                }
            }

        private static void DrawEnergyRect(ISpriteBatch spriteBatch, bool isGameRunningSlowly)
            {
            // outer frame
            var r = new Rectangle(22, 6, 148, 20);
            spriteBatch.DrawRectangle(r, isGameRunningSlowly ? Color.Red : Color.Blue);
            
            // clear inner area
            r.Inflate(-2, -2);
            spriteBatch.DrawRectangle(r, Color.Black);
            
            // always start off with energy bar in blue
            r = new Rectangle(32, 12, 128, 8);
            spriteBatch.DrawRectangle(r, Color.Blue);
            }
        
        private void DrawRemainingEnergy(ISpriteBatch spriteBatch, int energyToDisplay, int actualEnergy)
            {
            bool isAboutToDie = actualEnergy < 4;
            int barLength = isAboutToDie ? (energyToDisplay + 1) << 4 : Math.Min(energyToDisplay >> 2, 64);
            var barColour = isAboutToDie ? Color.Red : Color.Green;
            var r = new Rectangle(32, 12, barLength * 2, 8);
            spriteBatch.DrawRectangle(r, barColour);

#if DEBUG
            r = new Rectangle(168, 8, 28, 16);
            spriteBatch.DrawRectangle(r, Color.Black);
            DrawValue(spriteBatch, actualEnergy, 168 + 24, 8);
#endif
            }

        private static void DrawScoreAndLivesRect(ISpriteBatch spriteBatch)
            {
            var r = new Rectangle(342, 6, 148, 20);
            spriteBatch.DrawRectangle(r, Color.Blue);

            r.Inflate(-2, -2);
            spriteBatch.DrawRectangle(r, Color.Black);
            }

        private void DrawScore(ISpriteBatch spriteBatch, decimal score)
            {
            DrawValue(spriteBatch, score, 416, 8);
            }

        private void DrawLives(ISpriteBatch spriteBatch, int livesLeft)
            {
            if (livesLeft > 3)
                {
                spriteBatch.DrawTexture(this.Life, new Vector2(480 - 16, 8), new Rectangle(0, 0, 10, 16));
                spriteBatch.DrawTexture(this.Life, new Vector2(480 - 30, 8), new Rectangle(10, 0, 10, 16));
                DrawValue(spriteBatch, livesLeft + 100, 480 - 32, 8);
                }
            else
                {
                for (int i = 0; i < livesLeft; i++)
                    {
                    var destination = new Vector2(480 - ((i + 1) * 16), 8);
                    spriteBatch.DrawTexture(this.Life, destination, new Rectangle(0, 0, 10, 16));
                    }
                }
            }

        private void DrawValue(ISpriteBatch spriteBatch, decimal value, int right, int top)
            {
            int i = 1;
            while (true)
                {
                int digit = (int) (value % 10);
                var source = new Rectangle(digit * 6, 0, 6, 16);
                var destination = new Vector2(right - (i * 8), top);
                spriteBatch.DrawTexture(this.Digits, destination, source);
                value = Math.Floor(value / 10);
                if (value == 0)
                    break;
                i++;
                }
            }

        private void QueueStatusMessage(WorldStatus msg)
            {
            this._messages.Enqueue(msg.Message);
            }

        private abstract class MessageState
            {
            protected float TransitionPosition;
            protected abstract TimeSpan TransitionLength { get; }
            protected readonly HeadsUpDisplay Parent;
            protected const float Y = 350f;

            protected MessageState(HeadsUpDisplay headsUpDisplay)
                {
                this.Parent = headsUpDisplay ?? throw new ArgumentNullException(nameof(headsUpDisplay));
                }

            protected bool UpdateTransition(GameTime gameTime)
                {
                float transitionDelta;
                if (this.TransitionLength == TimeSpan.Zero)
                    transitionDelta = 1;
                else
                    transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / this.TransitionLength.TotalMilliseconds);

                this.TransitionPosition += transitionDelta;
                if (this.TransitionPosition >= 1f)
                    {
                    this.TransitionPosition = 1f;
                    return false;
                    }

                return true;
                }

            public abstract void Draw(GameTime gameTime, ISpriteBatch spriteBatch);
            }

        private class MessageFadeInState : MessageState
            {
            protected override TimeSpan TransitionLength => TimeSpan.FromSeconds(0.5);
            private readonly string _message;

            public MessageFadeInState(HeadsUpDisplay headsUpDisplay, string message) : base(headsUpDisplay)
                {
                this._message = message ?? throw new ArgumentNullException(nameof(message));
                }

            public override void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
                {
                if (!UpdateTransition(gameTime))
                    {
                    this.Parent._messageState = new MessageHeldState(this.Parent, this._message);
                    }
                var colour = new Color(0, this.TransitionPosition, 0, this.TransitionPosition);
                spriteBatch.DrawCentredString(this.Parent.StatusFont, this._message, Y, colour);
                }
            }

        private class MessageHeldState : MessageState
            {
            protected override TimeSpan TransitionLength => TimeSpan.FromSeconds(2);
            private readonly string _message;

            public MessageHeldState(HeadsUpDisplay headsUpDisplay, string message) : base(headsUpDisplay)
                {
                this._message = message ?? throw new ArgumentNullException(nameof(message));
                }

            public override void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
                {
                if (!UpdateTransition(gameTime))
                    {
                    if (this.Parent._messages.TryDequeue(out string? newMessage))
                        {
                        this.Parent._messageState = new MessageCrossFadeState(this.Parent, this._message, newMessage);
                        }
                    else
                        {
                        this.Parent._messageState = new MessageFadeOutState(this.Parent, this._message);
                        }
                    }
                var colour = new Color(0, 1f, 0, 1f);
                spriteBatch.DrawCentredString(this.Parent.StatusFont, this._message, Y, colour);
                }
            }

        private class MessageFadeOutState : MessageState
            {
            protected override TimeSpan TransitionLength => TimeSpan.FromSeconds(1);
            private readonly string _message;

            public MessageFadeOutState(HeadsUpDisplay headsUpDisplay, string message) : base(headsUpDisplay)
                {
                this._message = message ?? throw new ArgumentNullException(nameof(message));
                }

            public override void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
                {
                if (!UpdateTransition(gameTime))
                    {
                    this.Parent._messageState = null;
                    }
                var colour = new Color(0, 1f - this.TransitionPosition, 0, 1f - this.TransitionPosition);
                spriteBatch.DrawCentredString(this.Parent.StatusFont, this._message, Y, colour);
                }
            }

        private class MessageCrossFadeState : MessageState
            {
            protected override TimeSpan TransitionLength => TimeSpan.FromSeconds(1);
            private readonly string _oldMessage;
            private readonly string _newMessage;

            public MessageCrossFadeState(HeadsUpDisplay headsUpDisplay, string oldMessage, string newMessage) : base(headsUpDisplay)
                {
                this._oldMessage = oldMessage ?? throw new ArgumentNullException(nameof(oldMessage));
                this._newMessage = newMessage ?? throw new ArgumentNullException(nameof(newMessage));
                }

            public override void Draw(GameTime gameTime, ISpriteBatch spriteBatch)
                {
                if (!UpdateTransition(gameTime))
                    {
                    this.Parent._messageState = new MessageHeldState(this.Parent, this._newMessage);
                    }

                var fadeOutColour = new Color(0, 1f - this.TransitionPosition, 0, 1f - this.TransitionPosition);
                var fadeInColour = new Color(0, this.TransitionPosition, 0, this.TransitionPosition);

                var oldMessagePosition = new Vector2(spriteBatch.ScreenCentreWidth - this.TransitionPosition * 256, Y);
                var newMessagePosition = new Vector2(spriteBatch.ScreenCentreWidth + (1f - this.TransitionPosition) * 256, Y);
                var oldMessageOrigin = new Vector2(this.Parent.StatusFont.MeasureString(this._oldMessage).X / 2f, 0);
                var newMessageOrigin = new Vector2(this.Parent.StatusFont.MeasureString(this._newMessage).X / 2f, 0);

                spriteBatch.DrawString(this.Parent.StatusFont, this._oldMessage, oldMessagePosition, fadeOutColour, 0, oldMessageOrigin, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(this.Parent.StatusFont, this._newMessage, newMessagePosition, fadeInColour, 0, newMessageOrigin, 1, SpriteEffects.None, 0);
                }
            }
        }
    }
