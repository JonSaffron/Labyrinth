namespace Labyrinth
    {
    public interface IShot : IMovingItem
        {
        IGameObject Originator { get; }
        bool HasRebounded { get; }
        }
    }
