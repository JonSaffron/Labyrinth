using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth.ClockEvents
    {
    public class UnlockLevel : IClockEvent
        {
        private readonly World _world;
        private int _levelUnlocked;

        public UnlockLevel([NotNull] World world)
            {
            this._world = world ?? throw new ArgumentNullException(nameof(world));
            }

        public void Update(int ticks)
            {
            int maxLevelToUnlock = ticks >> 13;
            while (this._levelUnlocked < maxLevelToUnlock)
                {
                this._levelUnlocked++;
                Unlock(this._levelUnlocked);
                }
            }

        private void Unlock(int levelThatPlayerShouldHaveReached)
            {
            foreach (Monster m in GlobalServices.GameState.DistinctItemsOfType<Monster>())
                {
                if (m.IsEgg || m.IsActive || !m.IsExtant || m.ChangeRooms == ChangeRooms.StaysWithinRoom)
                    continue;

                int worldAreaId = this._world.GetWorldAreaIdForTilePos(m.TilePosition);
                if (worldAreaId < levelThatPlayerShouldHaveReached)
                    m.IsActive = true;
                }
            }
        }
    }
