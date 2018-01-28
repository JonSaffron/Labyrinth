using Labyrinth.GameObjects.Actions;

namespace Labyrinth.GameObjects
    {
    static class MonsterBehaviour
        {
        public static bool GetShootsAtPlayer(this Monster monster)
            {
            return monster.MovementBehaviours.Has<ShootsAtPlayer>();
            }

        public static void SetShootsAtPlayer(this Monster monster, bool value)
            {
            if (value)
                {
                var behaviour = new ShootsAtPlayer(new StandardMonsterWeapon(monster));
                behaviour.Init(monster);
                monster.MovementBehaviours.Add(behaviour);
                }
            else
                {
                monster.MovementBehaviours.Remove<ShootsAtPlayer>();
                }
            }

        public static bool GetShootsOnceProvoked(this Monster monster)
            {
            return monster.InjuryBehaviours.Has<StartsShootingWhenHurt>();
            }

        public static void SetShootsOnceProvoked(this Monster monster, bool value)
            { 
            monster.InjuryBehaviours.Set<StartsShootingWhenHurt>(value);
            }

        public static bool IsArmed(this IMonster monster)
            {
            var result = monster.HasBehaviour<ShootsAtPlayer>() || monster.HasBehaviour<StartsShootingWhenHurt>();
            return result;
            }
        }
    }
