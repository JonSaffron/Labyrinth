namespace Labyrinth.GameObjects.Monsters.Movement
    {
    class Stationary : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            return Direction.None;
            }
        }
    }
