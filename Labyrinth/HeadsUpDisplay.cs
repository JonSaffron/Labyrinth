using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class HeadsUpDisplay : IHeadsUpDisplay
        {
        private Texture2D _digits;
        private Texture2D _life;
        private SpriteFont _statusFont;
        private decimal _displayedScore;
        private int _displayedEnergy;
        private int _green;

        public void LoadContent(ContentManager contentManager)
            {
            this._digits = contentManager.Load<Texture2D>("Display/Digits");
            this._life = contentManager.Load<Texture2D>("Display/Life");
            this._statusFont = contentManager.Load<SpriteFont>("Display/StatusFont");
            }

        public void Reset()
            {
            this._displayedScore = 0;
            this._displayedEnergy = 0;
            }

        public void DrawStatus(ISpriteBatch spriteBatch, bool isPlayerExtant, int playerEnergy, decimal score, int livesLeft, bool isPaused, bool isGameRunningSlowly)
            {
            spriteBatch.Begin();

            if (!isPaused)
                {
                UpdateDisplayValues(playerEnergy, score);
                }

            DrawEnergyRect(spriteBatch, isGameRunningSlowly);
            if (isPlayerExtant)
                DrawRemainingEnergy(spriteBatch, this._displayedEnergy);

            DrawScoreAndLivesRect(spriteBatch);
            DrawScore(spriteBatch, this._displayedScore);
            DrawLives(spriteBatch, livesLeft);

            if (isPaused)
                {
                DrawPausedMessage(spriteBatch);
                }

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
        
        private void DrawRemainingEnergy(ISpriteBatch spriteBatch, int playerEnergy)
            {
            bool isAboutToDie = playerEnergy < 4;
            int barLength = isAboutToDie ? (playerEnergy + 1) << 4 : Math.Min(playerEnergy >> 2, 64);
            var barColour = isAboutToDie ? Color.Red : Color.Green;
            var r = new Rectangle(32, 12, barLength * 2, 8);
            spriteBatch.DrawRectangle(r, barColour);

#if DEBUG
            r = new Rectangle(168, 8, 28, 16);
            spriteBatch.DrawRectangle(r, Color.Black);
            DrawValue(spriteBatch, playerEnergy, 168 + 24, 8);
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
                spriteBatch.DrawTexture(this._life, new Vector2(480 - 16, 8), new Rectangle(0, 0, 10, 16));
                spriteBatch.DrawTexture(this._life, new Vector2(480 - 30, 8), new Rectangle(10, 0, 10, 16));
                DrawValue(spriteBatch, livesLeft + 100, 480 - 32, 8);
                }
            else
                {
                for (int i = 0; i < livesLeft; i++)
                    {
                    var destination = new Vector2(480 - ((i + 1) * 16), 8);
                    spriteBatch.DrawTexture(this._life, destination, new Rectangle(0, 0, 10, 16));
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
                spriteBatch.DrawTexture(this._digits, destination, source);
                value = Math.Floor(value / 10);
                if (value == 0)
                    break;
                i++;
                }
            }

        private void DrawPausedMessage(ISpriteBatch spriteBatch)
            {
            this._green = (this._green + 1) % 512;
            var greenComponent = Math.Abs(255 - this._green);
            const string paused = "-> P A U S E D <-";
            spriteBatch.DrawCentredString(this._statusFont, paused, 100, new Color(0, greenComponent, 0));
            }
        }
    }
