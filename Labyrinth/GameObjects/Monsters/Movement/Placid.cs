namespace Labyrinth.GameObjects.Movement
    {
    class Placid : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = MonsterMovement.RandomDirection();
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            return result;
            }
        }
    }
