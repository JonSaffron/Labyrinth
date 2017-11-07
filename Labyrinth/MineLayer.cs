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

        public void Fire(StaticItem source, FiringState firingState, Direction direction)
            {
            if (firingState != FiringState.Pulse || this._countOfMinesRemaining < 1)
                return;

            TilePos tp = source.TilePosition;
            if (GlobalServices.GameState.IsStaticItemOnTile(tp))
                return;

            GlobalServices.GameState.AddMine(tp.ToPosition());
            // todo play an appropriate sound here
            this._countOfMinesRemaining--;
            }
        }
    }
