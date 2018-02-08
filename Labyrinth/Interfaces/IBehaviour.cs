namespace Labyrinth
    {
    public interface IBehaviour
        {
        void Perform();
        }

    public interface IMovementBehaviour : IBehaviour
        {
        // marker interface
        }

    public interface IInjuryBehaviour : IBehaviour
        {
        // marker interface
        }

    public interface IDeathBehaviour : IBehaviour
        {
        // marker interface
        }
    }
