using System;
using System.Collections.Generic;

// http://blog.two-cats.com/2014/06/a-star-example/
// http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html
// https://blogs.msdn.microsoft.com/ericlippert/2007/10/page/2/

namespace Labyrinth.Services.PathFinder
    {
    public class PathFinder
        {
        private readonly PriorityQueue<float, ImmutableStack<Node>> _openNodes = new PriorityQueue<float, ImmutableStack<Node>>();
        private readonly HashSet<Node> _closedNodes = new HashSet<Node>();
        private readonly SearchParameters _searchParameters;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="searchParameters">An instance of a SearchParameters object that defines the search to perform</param>
        public PathFinder(SearchParameters searchParameters)
            {
            this._searchParameters = searchParameters;
            }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public List<TilePos> FindPath()
            {
            // The start node is the first entry in the 'open' list
            this._openNodes.Enqueue(0, new ImmutableStack<Node>(new Node(this._searchParameters.StartLocation)));

            while (!this._openNodes.IsEmpty)
                {
                var path = this._openNodes.Dequeue();

                if (this._closedNodes.Contains(path.LastStep))
                    continue;

                if (path.LastStep.Location == this._searchParameters.EndLocation)
                    {
                    var result = ExtractPath();
                    return result;
                    }

                this._closedNodes.Add(path.LastStep);


            IEnumerable<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);
            foreach (var nextNode in nextNodes)
                {
                this._nodes[nextNode.Location] = nextNode;
                }

            foreach (var nextNode in nextNodes)
                {
                this._nodes[nextNode.Location] = nextNode;

                // Check whether the end node has been reached
                if (nextNode.Location == this._searchParameters.EndLocation)
                    {
                    goal = nextNode;
                    return true;
                    }

                // If not, check the next set of nodes
                if (Search(nextNode, out goal)) // Note: Recurses back into Search(Node)
                    return true;
                }

            // The method returns false if this path leads to be a dead end
            goal = null;
            return false;
            }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private IEnumerable<Node> GetAdjacentWalkableNodes(Node fromNode)
            {
            IEnumerable<TilePos> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
                {
                // Ignore non-walkable nodes
                if (!this._searchParameters.CanBeOccupied(location))
                    continue;

                Node node = new Node(location) {ParentNode = fromNode};

                Node existingNode;
                if (this._nodes.TryGetValue(location, out existingNode) && node.G >= existingNode.G)
                    continue;

                node.H = GetEstimatedTraversalCost(node.Location);
                node.IsOpen = true;

                yield return node;
                }
            }

        /// <summary>
        /// Returns the four locations immediately orthogonally adjacent to <paramref name="fromLocation"/>
        /// </summary>
        /// <param name="fromLocation">The location from which to return all adjacent points</param>
        /// <returns>The locations as an IEnumerable of Points</returns>
        private static IEnumerable<TilePos> GetAdjacentLocations(TilePos fromLocation)
            {
            var result = new List<TilePos>(4);
            if (fromLocation.X > 0)
                result.Add(fromLocation.GetPositionAfterOneMove(Direction.Left));
            if (fromLocation.Y > 0)
                result.Add(fromLocation.GetPositionAfterOneMove(Direction.Up));
            result.Add(fromLocation.GetPositionAfterOneMove(Direction.Right));
            result.Add(fromLocation.GetPositionAfterOneMove(Direction.Down));
            return result;
            }

        /// <summary>
        /// Gets the estimated distance between a specifed point and the goal
        /// </summary>
        private float GetEstimatedTraversalCost(TilePos current)
            {
            var start = this._searchParameters.StartLocation;
            var goal = this._searchParameters.EndLocation;

            int deltaX = Math.Abs(goal.X - current.X);
            int deltaY = Math.Abs(goal.Y - current.Y);
            float result = (deltaX + deltaY);

            int dx1 = current.X - goal.X;
            int dy1 = current.Y - goal.Y;
            int dx2 = start.X - goal.X;
            int dy2 = start.Y - goal.Y;
            int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            result += cross * 0.001f;

            return result;
            }
        }
    }
