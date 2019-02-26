﻿namespace Labyrinth.GameObjects.Actions
    {
    class StartsShootingWhenHurt : BaseBehaviour, IInjuryBehaviour
        {
        public StartsShootingWhenHurt(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public StartsShootingWhenHurt()
            {
            // nothing to do
            }

        public override void Perform()
            {
            this.Monster.AddShootsAtPlayerBehaviour();
            this.Monster.Behaviours.Remove<StartsShootingWhenHurt>();
            }
        }
    }
