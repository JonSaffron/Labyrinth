using System.Collections.Generic;

// http://blog.two-cats.com/2014/06/a-star-example/
// http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html

namespace Labyrinth.Services.PathFinder
    {
    public class PathFinder
        {
        private readonly Dictionary<TilePos, Node> _nodes = new Dictionary<TilePos, Node>();
        private readonly Node _startNode;
        private readonly Node _endNode;
        private readonly SearchParameters _searchParameters;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="searchParameters"></param>
        public PathFinder(SearchParameters searchParameters)
            {
            this._searchParameters = searchParameters;
            this._startNode = GetNode(searchParameters.StartLocation);
            this._startNode.State = NodeState.Open;
            this._endNode = GetNode(searchParameters.EndLocation);
            }

        private Node GetNode(TilePos tp)
            {
            Node result;
            if (this._nodes.TryGetValue(tp, out result))
                return result;
            result = CreateNode(tp);
            this._nodes.Add(tp, result);
            return result;
            }

        private Node CreateNode(TilePos tp)
            {
            bool isWalkable = this._searchParameters.CanBeOccupied(tp);
            var result = new Node(tp, isWalkable, this._searchParameters.EndLocation);
            return result;
            }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public List<TilePos> FindPath()
            {
            // The start node is the first entry in the 'open' list
            List<TilePos> path = new List<TilePos>();
            bool success = Search(_startNode);
            if (success)
                {
                // If a path was found, follow the parents from the end node to build a list of locations
                Node node = this._endNode;
                while (node.ParentNode != null)
                    {
                    path.Add(node.Location);
                    node = node.ParentNode;
                    }

                // Reverse the list so it's in the correct order when returned
                path.Reverse();
                }

            return path;
            }

        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
        /// </summary>
        /// <param name="currentNode">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
        private bool Search(Node currentNode)
            {
            // Set the current node to Closed since it cannot be traversed more than once
            currentNode.State = NodeState.Closed;
            List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);

            // Sort by F-value so that the shortest possible routes are considered first
            nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach (var nextNode in nextNodes)
                {
                // Check whether the end node has been reached
                if (nextNode.Location == this._endNode.Location)
                    {
                    return true;
                    }

                // If not, check the next set of nodes
                if (Search(nextNode)) // Note: Recurses back into Search(Node)
                    return true;
                }

            // The method returns false if this path leads to be a dead end
            return false;
            }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private List<Node> GetAdjacentWalkableNodes(Node fromNode)
            {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<TilePos> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
                {
                Node node = GetNode(location);
                // Ignore non-walkable nodes
                if (!node.IsWalkable)
                    continue;

                // Ignore already-closed nodes
                if (node.State == NodeState.Closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (node.State == NodeState.Open)
                    {
                    float traversalCost = Node.GetTraversalCost(node.Location, fromNode.Location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                        {
                        node.ParentNode = fromNode;
                        walkableNodes.Add(node);
                        }
                    }
                else
                    {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.ParentNode = fromNode;
                    node.State = NodeState.Open;
                    walkableNodes.Add(node);
                    }
                }

            return walkableNodes;
            }

        /// <summary>
        /// Returns the eight locations immediately adjacent (orthogonally and diagonally) to <paramref name="fromLocation"/>
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
        }
    }
