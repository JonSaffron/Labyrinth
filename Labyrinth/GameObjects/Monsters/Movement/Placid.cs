namespace Labyrinth.GameObjects.Movement
    {
    class Placid : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            var result = MonsterMovement.RandomDirection();
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            return result;
            }
        }
    }
