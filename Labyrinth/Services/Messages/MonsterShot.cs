using System;

namespace Labyrinth.Services.Messages
    {
    internal class MonsterShot
        {
        public readonly IMonster Monster;
        public readonly IMunition Munition;

        public MonsterShot(IMonster monster, IMunition munition)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.Munition = munition ?? throw new ArgumentNullException(nameof(munition));
            }
        }
    }
