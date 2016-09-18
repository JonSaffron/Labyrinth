namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// Represents a single node on a grid that is being searched for a path between two points
    /// </summary>
    public class Node
        {
        private Node _parentNode;

        /// <summary>
        /// The node's location in the grid
        /// </summary>
        public TilePos Location { get; private set; }

        /// <summary>
        /// Cost from start to here
        /// </summary>
        public int G { get; private set; }

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
        /// Flags whether the node is open or closed by the PathFinder
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets the parent node. The start node's parent is always null.
        /// </summary>
        public Node ParentNode
            {
            get { return _parentNode; }
            set
                {
                _parentNode = value;
                this.G = (value == null) ? 0 : value.G + 1;
                }
            }

        /// <summary>
        /// Creates a new instance of Node.
        /// </summary>
        /// <param name="tp">The node's location</param>
        public Node(TilePos tp)
            {
            this.Location = tp;
            this.IsOpen = true;
            }

        public override string ToString()
            {
            return string.Format("{0}, {1}: {2}", this.Location.X, this.Location.Y, this.IsOpen);
            }
        }
    }
