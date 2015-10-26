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

        public void LoadContent(ContentManager contentManager)
            {
            this._digits = contentManager.Load<Texture2D>("Display/Digits");
            this._life = contentManager.Load<Texture2D>("Display/Life");
            }

        public void DrawStatus(ISpriteBatch spriteBatch, bool isPlayerExtant, int playerEnergy, int score, int livesLeft)
            {
            DrawEnergyRect(spriteBatch);
            if (isPlayerExtant)
                DrawEnergyBar(spriteBatch, playerEnergy);

            DrawScoreAndLivesRect(spriteBatch);
            DrawScore(spriteBatch, score);
            DrawLives(spriteBatch, livesLeft);
            }

        private static void DrawEnergyRect(ISpriteBatch spriteBatch)
            {
            var r = new Rectangle(22, 6, 148, 20);
            spriteBatch.DrawRectangle(r, Color.Blue);
            
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

        private void DrawScore(ISpriteBatch spriteBatch, int score)
            {
            DrawValue(spriteBatch, score, 416, 8);
            }

        private void DrawLives(ISpriteBatch spriteBatch, int livesLeft)
            {
            for (int i = 0; i < livesLeft; i++)
                {
                var destination = new Vector2(480 - ((i + 1) * 16), 8) + spriteBatch.WindowOffset;
                spriteBatch.DrawEntireTexture(this._life, destination);
                }
            }

        private void DrawValue(ISpriteBatch spriteBatch, int value, int right, int top)
            {
            int i = 1;
            while (true)
                {
                int digit = value % 10;
                var source = new Rectangle(digit * 6, 0, 6, 16);
                var destination = new Vector2(right - (i * 8), top) + spriteBatch.WindowOffset;
                spriteBatch.DrawTexture(this._digits, destination, source, 0.0f, Vector2.Zero);
                value = value / 10;
                if (value == 0)
                    break;
                i++;
                }
            }
        }
    }
