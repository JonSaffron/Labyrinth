using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Monsters.Actions
    {
    class ShootsAtPlayer : BaseAction
        {
        public override void PerformAction()
            {
            if (!ShouldAttemptToFireAtPlayer())
                return;

            this.Monster.FireWeapon();
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
