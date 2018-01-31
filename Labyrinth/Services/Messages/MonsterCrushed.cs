using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    class MonsterCrushed
        {
        [NotNull] public readonly IMonster Monster;
        [NotNull] public readonly IGameObject CrushedBy;

        public MonsterCrushed([NotNull] IMonster monster, [NotNull] IGameObject crushedBy)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            this.CrushedBy = crushedBy ?? throw new ArgumentNullException(nameof(crushedBy));
            }
        }
    }
