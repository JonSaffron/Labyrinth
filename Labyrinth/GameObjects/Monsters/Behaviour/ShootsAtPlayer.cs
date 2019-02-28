using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    class ShootsAtPlayer : BaseBehaviour, IMovementBehaviour
        {
        public ShootsAtPlayer()
            {

            }

        public ShootsAtPlayer(Monster monster) : base(monster)
            {
            OnInit();
            }

        protected sealed override void OnInit()
            {
            this.Monster.Weapon = new StandardMonsterWeapon(this.Monster);
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
