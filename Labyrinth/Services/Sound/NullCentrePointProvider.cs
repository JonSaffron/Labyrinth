using Microsoft.Xna.Framework;

namespace Labyrinth.Services.Sound
    {
    internal class NullCentrePointProvider : ICentrePointProvider
        {
        public Vector2 GetDistanceFromCentreOfScreen(Vector2 _)
            {
            return Vector2.Zero;
            }
        }
    }
