using System;

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
        /// True when the node may be traversed, otherwise false
        /// </summary>
        public bool IsWalkable { get; set; }
        
        /// <summary>
        /// Cost from start to here
        /// </summary>
        public float G { get; private set; }

        /// <summary>
        /// Estimated cost from here to end
        /// </summary>
        public float H { get; private set; }

        /// <summary>
        /// Flags whether the node is open, closed or untested by the PathFinder
        /// </summary>
        public NodeState State { get; set; }

        /// <summary>
        /// Estimated total cost (F = G + H)
        /// </summary>
        public float F
            {
            get { return this.G + this.H; }
            }

        /// <summary>
        /// Gets or sets the parent node. The start node's parent is always null.
        /// </summary>
        public Node ParentNode
            {
            get { return this._parentNode; }
            set
                {
                // When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
                this._parentNode = value;
                this.G = this._parentNode.G + GetTraversalCost(this.Location, this._parentNode.Location);
                }
            }

        /// <summary>
        /// Creates a new instance of Node.
        /// </summary>
        /// <param name="tp">The node's location</param>
        /// <param name="isWalkable">True if the node can be traversed, false if the node is a wall</param>
        /// <param name="endLocation">The location of the destination node</param>
        public Node(TilePos tp, bool isWalkable, TilePos endLocation)
            {
            this.Location = tp;
            this.State = NodeState.Untested;
            this.IsWalkable = isWalkable;
            this.H = GetTraversalCost(this.Location, endLocation);
            this.G = 0;
            }

        public override string ToString()
            {
            return string.Format("{0}, {1}: {2}", this.Location.X, this.Location.Y, this.State);
            }

        /// <summary>
        /// Gets the distance between two points
        /// </summary>
        internal static float GetTraversalCost(TilePos current, TilePos start, TilePos goal)
            {
            int deltaX = Math.Abs(goal.X - current.X);
            int deltaY = Math.Abs(goal.Y - current.Y);
            float result = (deltaX + deltaY);

            int dx1 = current.X - goal.X;
            int dy1 = current.Y - goal.Y;
            int dx2 = start.X - goal.X;
            int dy2 = start.Y - goal.Y;
            int cross = Math.Abs(dx1*dy2 - dx2*dy1);
            result += cross * 0.001f;

            return result;
            }
        }
    }
