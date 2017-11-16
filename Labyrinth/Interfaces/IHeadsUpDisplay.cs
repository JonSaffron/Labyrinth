using Microsoft.Xna.Framework.Content;

namespace Labyrinth
    {
    internal interface IHeadsUpDisplay
        {
        void LoadContent(ContentManager contentManager);
        void Reset();
        void DrawStatus(ISpriteBatch spriteBatch, bool isPlayerExtant, int playerEnergy, decimal score, int livesLeft, bool isPaused, bool isRunningSlowly);
        }
    }
