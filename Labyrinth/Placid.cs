using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class Placid : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            var result = MonsterMovement.RandomDirection();
            return result;
            }
        }
    }