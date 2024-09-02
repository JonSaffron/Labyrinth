using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Labyrinth
    {
    internal interface IHeadsUpDisplay
        {
        void LoadContent(ContentManager contentManager);
        void Reset();
        void DrawStatus(ISpriteBatch spriteBatch, GameTime gameTime, bool isPlayerExtant, int playerEnergy, decimal score, int livesLeft);
        }
    }
