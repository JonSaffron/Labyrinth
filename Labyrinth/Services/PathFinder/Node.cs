namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// Represents a single node on a grid that is being searched for a path between two points
    /// </summary>
    public class Node
        {
        /// <summary>
        /// The node's location in the grid
        /// </summary>
        public TilePos Location { get; private set; }

        /// <summary>
        /// Cost from start to here
        /// </summary>
        public int G { get; set; }

        /// <summary>
        /// Estimated cost from here to end
        /// </summary>
        public float H { get; set; }

        /// <summary>
        /// Estimated total cost (F = G + H)
        /// </summary>
        public float F
            {
            get { return this.G + this.H; }
            }

        /// <summary>
        /// Creates a new instance of Node.
        /// </summary>
        /// <param name="tp">The node's location</param>
        public Node(TilePos tp)
            {
            this.Location = tp;
            }

        public override string ToString()
            {
            return string.Format("{0}, {1}: {2}", this.Location.X, this.Location.Y, this.F);
            }
        }
    }
