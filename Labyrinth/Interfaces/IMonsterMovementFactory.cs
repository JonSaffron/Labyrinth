namespace Labyrinth
    {
    internal interface IMonsterMovementFactory
        {
        IMonsterMovement StandardPatrolling(Direction initialDirection);
        IMonsterMovement StandardRolling(Direction initialDirection);
        IMonsterMovement FullPursuit();
        IMonsterMovement Cautious();
        IMonsterMovement SemiAggressive();
        IMonsterMovement Placid();
        IMonsterMovement KillerCubeRedMovement();
        IMonsterMovement RotaFloaterCyanMovement();
        }
    }