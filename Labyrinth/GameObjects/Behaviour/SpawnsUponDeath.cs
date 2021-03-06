﻿namespace Labyrinth.GameObjects.Behaviour
    {
    class SpawnsUponDeath : BaseBehaviour, IDeathBehaviour
        {
        public SpawnsUponDeath(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public SpawnsUponDeath()
            {
            // nothing to do
            }

        public override void Perform()
            {
            var newDiamondDemon = GlobalServices.GameState.AddDiamondDemon(this.Monster.Position);
            GlobalServices.GameState.AddDiamondDemon(this.Monster.Position);
            newDiamondDemon.PlaySound(GameSound.MonsterShattersIntoNewLife);
            }
        }
    }
