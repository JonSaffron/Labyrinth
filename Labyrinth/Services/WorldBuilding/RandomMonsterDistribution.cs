using System.Collections.Generic;

namespace Labyrinth.Services.WorldBuilding
    {
    class RandomMonsterDistribution
        {
        private readonly Dictionary<int, MonsterDef> _monsterTemplates = new Dictionary<int, MonsterDef>();

        public DiceRoll DiceRoll { get; set; }
        public int CountOfMonsters { get; set; }

        public Dictionary<int, MonsterDef> Templates
            {
            get { return this._monsterTemplates; }
            }
        }
    }
