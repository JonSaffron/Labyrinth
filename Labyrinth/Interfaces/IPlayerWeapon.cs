using Labyrinth.GameObjects;

namespace Labyrinth
    {
    interface IPlayerWeapon
        {
        void Reset();
        void Fire(StaticItem source, FiringState firingState, Direction direction);
        }
    }
