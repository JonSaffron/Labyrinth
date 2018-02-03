using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    class MonsterShot
        {
        public readonly IMonster Monster;
        public readonly IShot Shot;

        public MonsterShot([NotNull] IMonster monster, [NotNull] IShot shot)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.Shot = shot ?? throw new ArgumentNullException(nameof(shot));
            }
        }
    }
