namespace Labyrinth
    {
    internal interface IMonsterMovementFactory
        {
        IMonsterMovement StandardPatrolling(Direction initialDirection);
        IMonsterMovement StandardRolling(Direction inititalDirection);
        IMonsterMovement FullPursuit();
        IMonsterMovement Cautious();
        IMonsterMovement SemiAggressive();
        IMonsterMovement Placid();
        IMonsterMovement KillerCubeRedMovement();
        IMonsterMovement RotaFloaterCyanMovement();
        }
    }