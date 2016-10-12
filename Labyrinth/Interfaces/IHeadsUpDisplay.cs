using Microsoft.Xna.Framework.Content;

namespace Labyrinth
    {
    internal interface IHeadsUpDisplay
        {
        void LoadContent(ContentManager contentManager);
        void DrawStatus(ISpriteBatch spriteBatch, bool isPlayerExtant, int playerEnergy, int score, int livesLeft);
        void DrawPausedMessage(ISpriteBatch spriteBatch);
        }
    }
