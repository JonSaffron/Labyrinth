namespace Labyrinth.DataStructures
    {
    /// <summary>
    /// Encapsulates a planned direction to move in
    /// </summary>
    public interface IDirectionChosen
        {
        /// <summary>
        /// The direction of travel
        /// </summary>
        Direction Direction { get; }
        }

    /// <summary>
    /// Encapsulates a possible direction to move in
    /// </summary>
    public readonly struct PossibleDirection : IDirectionChosen
        {
        /// <summary>
        /// A possible movement Left
        /// </summary>
        public static PossibleDirection Left = new PossibleDirection(Direction.Left);

        /// <summary>
        /// A possible movement right
        /// </summary>
        public static PossibleDirection Right = new PossibleDirection(Direction.Right);

        /// <summary>
        /// A possible movement up
        /// </summary>
        public static PossibleDirection Up = new PossibleDirection(Direction.Up);

        /// <summary>
        /// A possible movement down
        /// </summary>
        public static PossibleDirection Down = new PossibleDirection(Direction.Down);

        /// <summary>
        /// Constructs a possible movement in the specified direction
        /// </summary>
        /// <param name="direction">The possible direction to move in</param>
        public PossibleDirection(Direction direction)
            {
            this.Direction = direction;
            }

        /// <summary>
        /// Gets the possible direction of movement
        /// </summary>
        public Direction Direction { get; }
        }

    /// <summary>
    /// Encapsulates a confirmed direction to move in
    /// </summary>
    public readonly struct ConfirmedDirection : IDirectionChosen
        {
        /// <summary>
        /// Gets a stationary move
        /// </summary>
        public static readonly ConfirmedDirection None = new ConfirmedDirection(Direction.None);

        /// <summary>
        /// Constructs a confirmed movement in the specified direction
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        public ConfirmedDirection(Direction direction) : this()
            {
            this.Direction = direction;
            }

        /// <summary>
        /// Gets the confirmed direction of movement
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Implicit conversion to Direction
        /// </summary>
        /// <param name="direction"></param>
        public static implicit operator Direction(ConfirmedDirection direction) => direction.Direction;
        }

    /// <summary>
    /// Helper class for extension methods
    /// </summary>
    public static class DirectionChosenExtension
        {
        /// <summary>
        /// Creates a confirmed direction from the specified 
        /// </summary>
        /// <param name="directionChosen"></param>
        /// <returns>A confirmed direction of travel</returns>
        public static ConfirmedDirection Confirm(this IDirectionChosen directionChosen)
            {
            return new ConfirmedDirection(directionChosen.Direction);
            }
        }
    }
