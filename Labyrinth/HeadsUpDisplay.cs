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

        public void DrawStatus(ISpriteBatch spriteBatch, bool isPlayerExtant, int playerEnergy, decimal score, int livesLeft, bool isGameRunningSlowly)
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

            DrawEnergyRect(spriteBatch, isGameRunningSlowly);
            if (isPlayerExtant)
                DrawEnergyBar(spriteBatch, this._displayedEnergy);

            DrawScoreAndLivesRect(spriteBatch);
            DrawScore(spriteBatch, this._displayedScore);
            DrawLives(spriteBatch, livesLeft);
            }

        private static void DrawEnergyRect(ISpriteBatch spriteBatch, bool isGameRunningSlowly)
            {
            var r = new Rectangle(22, 6, 148, 20);
            spriteBatch.DrawRectangle(r, isGameRunningSlowly ? Color.Red : Color.Blue);
            
            r.Inflate(-2, -2);
            spriteBatch.DrawRectangle(r, Color.Black);
            
            r = new Rectangle(32, 12, 128, 8);
            spriteBatch.DrawRectangle(r, Color.Blue);
            }
        
        private void DrawEnergyBar(ISpriteBatch spriteBatch, int playerEnergy)
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
            for (int i = 0; i < livesLeft; i++)
                {
                var destination = new Vector2(480 - ((i + 1) * 16), 8);
                spriteBatch.DrawEntireTexture(this._life, destination);
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

        // todo this shouldn't need to know anything about zoom
        public void DrawPausedMessage(ISpriteBatch spriteBatch)
            {
            this._green = (this._green + 1) % 512;
            var greenComponent = Math.Abs(255 - this._green);
            const string paused = "-> P A U S E D <-";
            Vector2 size = this._statusFont.MeasureString(paused) * spriteBatch.Zoom;
            Vector2 origin = Vector2.Zero;
            Vector2 pos = new Vector2(Constants.RoomSizeInPixels.X * spriteBatch.Zoom / 2f - size.X / 2f, 200);
            spriteBatch.DrawString(this._statusFont, paused, pos, new Color(0, greenComponent, 0), origin);
            }
        }
    }
