using Labyrinth.GameObjects.Behaviour;

namespace Labyrinth.GameObjects
    {
    static class MonsterBehaviour
        {
        // todo refactor - check Weapon instead?
        public static bool IsArmed(this IMonster monster)
            {
            var result = monster.HasBehaviour<ShootsAtPlayer>() || monster.HasBehaviour<StartsShootingWhenHurt>();
            return result;
            }
        }
    }
