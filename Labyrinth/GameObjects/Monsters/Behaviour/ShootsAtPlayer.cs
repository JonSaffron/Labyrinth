using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    class ShootsAtPlayer : BaseBehaviour, IMovementBehaviour
        {
        public ShootsAtPlayer(Monster monster) : base(monster)
            {
            this.Monster.Weapon = new StandardMonsterWeapon(monster);
            }

        public override void Perform()
            {
            if (!ShouldAttemptToFireAtPlayer())
                return;

            this.Monster.Weapon?.FireIfYouLike();
            }

        private bool ShouldAttemptToFireAtPlayer()
            {
            var result =
                !this.Monster.IsEgg
                && this.IsInSameRoom()
                && !this.Random.Test(0x03) // 3 in 4 chance is deliberate
                && this.Player.IsAlive()
                && MonsterMovement.IsPlayerInWeaponSights(this.Monster);
            return result;
            }
        }
    }
