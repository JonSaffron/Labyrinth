using Labyrinth.GameObjects;

namespace Labyrinth
    {
    interface IMonsterMovement
        {
        Direction DetermineDirection(Monster monster);
        }
    }
