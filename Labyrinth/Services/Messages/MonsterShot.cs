using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    class MonsterShot
        {
        public readonly IMonster Monster;
        public readonly IMunition Munition;

        public MonsterShot([NotNull] IMonster monster, [NotNull] IMunition munition)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.Munition = munition ?? throw new ArgumentNullException(nameof(munition));
            }
        }
    }
