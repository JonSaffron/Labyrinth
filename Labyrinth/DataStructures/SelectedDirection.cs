namespace Labyrinth.DataStructures
    {
    public readonly struct SelectedDirection
        {
        public readonly Direction Direction;
        public readonly bool IsConfirmedSafe;

        private SelectedDirection(Direction direction, bool isConfirmedSafe)
            {
            this.Direction = direction;
            this.IsConfirmedSafe = isConfirmedSafe;
            }

        public static SelectedDirection SafeDirection(Direction direction)
            {
            return new SelectedDirection(direction, true);
            }

        public static SelectedDirection UnsafeDirection(Direction direction)
            {
            return new SelectedDirection(direction, false);
            }
        }
    }
