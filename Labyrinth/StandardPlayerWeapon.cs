using System;
using System.Linq;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class StandardPlayerWeapon : IPlayerWeapon
        {
        private int _countOfShotsBeforeCostingEnergy;

        public void Reset()
            {
            this._countOfShotsBeforeCostingEnergy = 0;    // first shot will cost energy
            }

        public void Fire(StaticItem source, FiringState firingState, Direction direction)
            {
            if (direction == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(direction));

            if (firingState != FiringState.Pulse || source.Energy < 4)
                return;

            var adjacentTilePos = source.TilePosition.GetPositionAfterOneMove(direction);
            var itemsOnTile = GlobalServices.GameState.GetItemsOnTile(adjacentTilePos);
            if (itemsOnTile.Any(item => item.Properties.Get(GameObjectProperties.EffectOfShot) == EffectOfShot.Impervious))
                {
                return;
                }

            var startPos = source.TilePosition.ToPosition() + direction.ToVector() * Constants.CentreOfTile;
            GlobalServices.GameState.AddStandardShot(startPos, direction, source.Energy >> 2, source);

            source.PlaySound(GameSound.PlayerShoots);
            _countOfShotsBeforeCostingEnergy--;
            if (_countOfShotsBeforeCostingEnergy < 0)
                {
                _countOfShotsBeforeCostingEnergy = 3;
                source.ReduceEnergy(1);
                }
            }
        }
    }
