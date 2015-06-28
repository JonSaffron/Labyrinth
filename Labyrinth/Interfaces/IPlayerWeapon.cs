namespace Labyrinth
    {
    interface IPlayerWeapon
        {
        void Reset();
        void Fire(StaticItem source, World world, FiringState firingState, Direction direction);
        }
    }
