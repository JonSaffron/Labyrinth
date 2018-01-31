using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.Messages
    {
    class MonsterShot
        {
        public readonly IMonster Monster;
        public readonly Shot Shot;

        public MonsterShot([NotNull] IMonster monster, [NotNull] Shot shot)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.Shot = shot ?? throw new ArgumentNullException(nameof(shot));
            }
        }
    }
