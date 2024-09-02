using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    internal class MonsterCrushed
        {
        public readonly IMonster Monster;
        [PublicAPI] public readonly IGameObject CrushedBy;

        public MonsterCrushed(IMonster monster, IGameObject crushedBy)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.CrushedBy = crushedBy ?? throw new ArgumentNullException(nameof(crushedBy));
            }
        }
    }
