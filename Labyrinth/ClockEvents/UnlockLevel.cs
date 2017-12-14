using Labyrinth.GameObjects;

namespace Labyrinth.ClockEvents
    {
    public class UnlockLevel : IClockEvent
        {
        private int _levelUnlocked;

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
            World world = GlobalServices.World;

            foreach (Monster m in GlobalServices.GameState.DistinctItemsOfType<Monster>())
                {
                if (m.IsEgg || m.IsActive || !m.IsExtant || m.ChangeRooms == ChangeRooms.StaysWithinRoom)
                    continue;

                int worldAreaId = world.GetWorldAreaIdForTilePos(m.TilePosition);
                if (worldAreaId < levelThatPlayerShouldHaveReached)
                    m.IsActive = true;
                }
            }
        }
    }