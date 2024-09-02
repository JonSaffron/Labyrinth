using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface ICentrePointProvider
        {
        Vector2 GetDistanceFromCentreOfScreen(Vector2 gameObjectPosition);
        }
    }
