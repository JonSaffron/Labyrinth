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
            var block = this._movingItem as Boulder;
            if (block != null)
                {
                block.Push(this._shot.Direction, false);
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            if (this._movingItem is Player)
                {
                this._movingItem.ReduceEnergy(this._shot.Energy);
                if (this._movingItem.IsAlive())
                    this._world.Game.SoundLibrary.Play(GameSound.PlayerInjured);
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            var monster = this._movingItem as Monster.Monster;
            if (monster != null)
                {
                if (!monster.IsActive)
                    monster.IsActive = true;
            
                if (monster.Mobility == MonsterMobility.Patrolling)
                    monster.Mobility = MonsterMobility.Placid;
            
                if (monster.ShotsBounceOff)
                    {
                    if (!this._shot.HasRebounded)
                        this._shot.Reverse();
                    return;
                    }
            
                var score = ((Math.Min(this._shot.Energy, monster.Energy) >> 1) + 1) * 10;
                this._world.IncreaseScore(score);
                monster.ReduceEnergy(this._shot.Energy);
                if (monster.IsAlive())
                    {
                    var sound = monster.IsEgg ? GameSound.PlayerShootsAndInjuresEgg : GameSound.PlayerShootsAndInjuresMonster;
                    this._world.Game.SoundLibrary.Play(sound);
                    }
                this._world.ConvertShotToBang(this._shot);
                return;
                }

            var shot = this._movingItem as Shot;
            if (shot != null && shot.ShotType != this._shot.ShotType && shot.Direction == this._shot.Direction.Reversed())
                {
                int minEnergy = Math.Min(shot.Energy, this._shot.Energy);
                this._world.Game.SoundLibrary.Play(GameSound.StaticObjectShotAndInjured);
                shot.ReduceEnergy(minEnergy);
                if (!shot.IsExtant)
                    this._world.ConvertShotToBang(shot);
                this._shot.ReduceEnergy(minEnergy);
                if (!this._shot.IsExtant)
                    this._world.ConvertShotToBang(this._shot);
                }

            // todo: add tiles?

            }
        }
    }
