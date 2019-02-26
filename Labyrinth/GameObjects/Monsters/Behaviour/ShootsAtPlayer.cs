using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    class ShootsAtPlayer : BaseBehaviour, IMovementBehaviour
        {
        private readonly IMonsterWeapon _weapon;

        public ShootsAtPlayer(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public ShootsAtPlayer()
            {
            // nothing to do
            }

        public ShootsAtPlayer([NotNull] IMonsterWeapon weapon)
            {
            this._weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
            }

        public override void Perform()
            {
            if (!ShouldAttemptToFireAtPlayer())
                return;

            this._weapon.FireIfYouLike();
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
