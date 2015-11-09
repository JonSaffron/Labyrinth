using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public interface IMonsterMovement
        {
        Direction DetermineDirection(Monster monster);
        }
    }
