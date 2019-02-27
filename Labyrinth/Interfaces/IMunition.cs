namespace Labyrinth
    {
    public interface IMunition : IMovingItem
        {
        IGameObject Originator { get; }
        }
    }
