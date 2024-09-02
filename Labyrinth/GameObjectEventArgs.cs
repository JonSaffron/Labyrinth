using System;

namespace Labyrinth
    {
    public class GameObjectEventArgs : EventArgs
        {
        public GameObjectEventArgs(IGameObject gameObject)
            {
            this.GameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            }

        public IGameObject GameObject { get; }
        }
    }
