using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class MineLayer : IPlayerWeapon
        {
        private int _countOfMinesRemaining;

        public void Reset()
            {
            this._countOfMinesRemaining = 10;
            }

        public void Fire(StaticItem source, World world, FiringState firingState, Direction direction)
            {
            if (firingState != FiringState.Pulse || this._countOfMinesRemaining < 1)
                return;

            TilePos tp = source.TilePosition;
            if (world.IsStaticItemOnTile(tp))
                return;

            world.AddMine(tp.ToPosition());
            this._countOfMinesRemaining--;
            }
        }
    }
