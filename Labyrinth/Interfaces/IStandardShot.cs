namespace Labyrinth
    {
    public interface IStandardShot : IMunition
        {
        bool HasRebounded { get; }
        void Reverse();
        }
    }
