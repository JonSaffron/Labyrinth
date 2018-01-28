using Labyrinth.GameObjects;

namespace Labyrinth.Services.Messages
    {
    class MonsterCrushed
        {
        public IMonster Monster;
        public MovingItem CrushedBy;
        public int EnergyRemoved;
        }
    }
