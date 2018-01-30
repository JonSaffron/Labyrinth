using System;
using JetBrains.Annotations;

namespace Labyrinth
    {
    public class GameObjectEventArgs : EventArgs
        {
        public GameObjectEventArgs([NotNull] IGameObject gameObject)
            {
            this.GameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            }

        public IGameObject GameObject { get; }
        }
    }
