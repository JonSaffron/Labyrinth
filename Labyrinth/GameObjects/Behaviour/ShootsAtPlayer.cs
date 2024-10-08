﻿using JetBrains.Annotations;
using Labyrinth.GameObjects.Motility;

namespace Labyrinth.GameObjects.Behaviour
    {
    internal class ShootsAtPlayer : BaseBehaviour, IMovementBehaviour
        {
        public ShootsAtPlayer()
            {
            // nothing to do
            }

        [UsedImplicitly]
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
                this.IsInSameRoom()
                && !this.Random.Test(0x03) // 3 in 4 chance is deliberate
                && this.Player.IsAlive()
                && this.Monster.IsPlayerInWeaponSights();
            return result;
            }
        }
    }
