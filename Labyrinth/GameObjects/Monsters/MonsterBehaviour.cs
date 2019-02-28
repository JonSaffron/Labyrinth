using Labyrinth.GameObjects.Behaviour;

namespace Labyrinth.GameObjects
    {
    static class MonsterBehaviour
        {
        public static void AddShootsAtPlayerBehaviour(this Monster monster)
            {
            var behaviour = new ShootsAtPlayer(monster);
            monster.Behaviours.Add(behaviour);
            }

        public static void SetShootsAtPlayer(this Monster monster, bool value)
            {
            if (value)
                {
                AddShootsAtPlayerBehaviour(monster);
                }
            else
                {
                monster.Behaviours.Remove<ShootsAtPlayer>();
                }
            }

        public static bool IsArmed(this IMonster monster)
            {
            var result = monster.HasBehaviour<ShootsAtPlayer>() || monster.HasBehaviour<StartsShootingWhenHurt>();
            return result;
            }
        }
    }
