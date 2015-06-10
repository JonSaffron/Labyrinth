using System;

namespace Labyrinth
    {
    class ShotAndMovingItemInteraction : IInteraction
        {
        private readonly World _world;
        private readonly Shot _shot;
        private readonly MovingItem _movingItem;

        public ShotAndMovingItemInteraction(World world, Shot shot, MovingItem movingItem)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (shot == null)
                throw new ArgumentNullException("shot");
            if (movingItem == null)
                throw new ArgumentNullException("movingItem");

            this._world = world;
            this._shot = shot;
            this._movingItem = movingItem;
            }

        public void Collide()
            {
            if (!this._shot.IsExtant || !this._movingItem.IsExtant)
                return;

            // block, player, shot, monster (death cube & crazy crawler)
#warning this should really be checking the object capabilities
            var block = this._movingItem as Boulder;
            if (block != null)
                {
                block.Push(this._shot.Direction);
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            if (this._movingItem is Player)
                {
                this._movingItem.ReduceEnergy(this._shot.Energy);
                if (this._movingItem.IsAlive())
                    this._world.Game.SoundPlayer.Play(GameSound.PlayerInjured);
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            var mine = this._movingItem as Mine;
            if (mine != null)
                {
                mine.SteppedOnBy(this._shot);
                this._shot.InstantlyExpire();
                return;
                }

            var monster = this._movingItem as Monster.Monster;
            if (monster != null)
                {
                if (!monster.IsActive)
                    monster.IsActive = true;
            
                if (monster.Mobility == MonsterMobility.Patrolling)
                    monster.Mobility = MonsterMobility.Placid;
            
                var standardShot = this._shot as StandardShot;
                if (standardShot != null && monster.ShotsBounceOff)
                    {
                    if (!standardShot.HasRebounded)
                        standardShot.Reverse();
                    return;
                    }
            
                var score = ((Math.Min(this._shot.Energy, monster.Energy) >> 1) + 1) * 10;
                this._world.IncreaseScore(score);
                monster.ReduceEnergy(this._shot.Energy);
                if (monster.IsAlive())
                    {
                    var sound = monster.IsEgg ? GameSound.PlayerShootsAndInjuresEgg : GameSound.PlayerShootsAndInjuresMonster;
                    this._world.Game.SoundPlayer.Play(sound);
                    }
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            var standardShot1 = this._shot as StandardShot;
            var standardShot2 = this._movingItem as StandardShot;
            if (standardShot1 != null && standardShot2 != null 
                && standardShot2.ShotType != standardShot1.ShotType 
                && standardShot2.Direction == standardShot1.Direction.Reversed())
                {
                int minEnergy = Math.Min(standardShot2.Energy, standardShot1.Energy);
                this._world.Game.SoundPlayer.Play(GameSound.StaticObjectShotAndInjured);
                standardShot2.ReduceEnergy(minEnergy);
                if (!standardShot2.IsExtant)
                    this._world.ConvertShotToBang(standardShot2);
                standardShot1.ReduceEnergy(minEnergy);
                if (!standardShot1.IsExtant)
                    this._world.ConvertShotToBang(standardShot1);
                }

            // todo: add tiles?

            }
        }
    }
