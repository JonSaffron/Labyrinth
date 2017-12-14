using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public class GameObjectEventArgs : EventArgs
        {
        public GameObjectEventArgs([NotNull] StaticItem gameObject)
            {
            this.GameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            }

        public StaticItem GameObject { get; }
        }
    }
