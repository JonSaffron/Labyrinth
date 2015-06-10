using System;

namespace Labyrinth
    {
    class StandardPlayerWeapon : IPlayerWeapon
        {
        private int _countOfShotsBeforeCostingEnergy;

        public void Reset()
            {
            this._countOfShotsBeforeCostingEnergy = 0;    // first shot will cost energy
            }

        public void Fire(StaticItem source, World world, FiringState firingState, Direction direction)
            {
            if (direction == Direction.None)
                throw new ArgumentOutOfRangeException("direction");

            if (firingState != FiringState.Pulse || source.Energy < 4)
                return;
            var startPos = source.TilePosition.ToPosition();
            var shot = new StandardShot(world, startPos, direction, source.Energy >> 2, ShotType.Player);
            if (!shot.CanMoveInDirection(direction))
                return;
            startPos += direction.ToVector() * Tile.Size / 2;
            shot.SetPosition(startPos);

            world.AddShot(shot);
            world.Game.SoundPlayer.Play(GameSound.PlayerShoots);
            _countOfShotsBeforeCostingEnergy--;
            if (_countOfShotsBeforeCostingEnergy < 0)
                {
                _countOfShotsBeforeCostingEnergy = 3;
                source.ReduceEnergy(1);
                }
            }
        }
    }
