using System.Collections.Generic;

namespace ZB.Gameplay
{
    public class AStarPathFinder : PathFinder
    {
        #region Class Methods

        public override List<Node> Find(Node[,] nodes, Node startNode, Node endNode)
        {
            PriorityQueue openSet = new PriorityQueue(nodes.GetLength(0) * nodes.GetLength(1));
            HashSet<Node> closedSet = new HashSet<Node>();

            // Add the only known start node to the open set.
            openSet.AddANode(startNode);

            // Loop until the open set is empty (meaning there is no path) or when we reach the end node (meaning we have found a path).
            while (!openSet.IsEmpty())
            {
                // Get the node with the minimum cost out of the open set.
                AStarNode currentNode = openSet.GetMinimumCostNode() as AStarNode;

                // If the current node with the minimum cost is also the end node => break out of the loop, because we have our path found.
                if (currentNode == endNode)
                    break;

                // Add this current node to the closed set, we won't re-examine those nodes again.
                closedSet.Add(currentNode);

                // With each neighbour node around the current node, we have to examine each of them.
                foreach (AStarNode neighbourNode in MapUtils.GetNeighbourNodes(nodes, currentNode))
                {
                    // With the nodes that are untraversable or have already been in the closed set, we won't do anything with them.
                    if (!neighbourNode.IsTraversable || closedSet.Contains(neighbourNode))
                        continue;

                    // Calculate the gCost from the current node through the neighbour node.
                    int newGCostToNeighbourNode = currentNode.GCost + MapUtils.GetHeuristicDistanceBetweenTwoNodes(currentNode, neighbourNode);

                    // If the new gCost of this neighbour node is lower than its previous gCost or it's not in the open set.
                    if (newGCostToNeighbourNode < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                    {
                        // Update data for this neighbour node.
                        neighbourNode.GCost = newGCostToNeighbourNode;
                        neighbourNode.HCost = MapUtils.GetHeuristicDistanceBetweenTwoNodes(neighbourNode, endNode);

                        // Set its parent to the current node for retracing.
                        neighbourNode.ParentNode = currentNode;

                        // If the neighbour node is not yet in the open set, then add it to the open set for later exploration, otherwise, update it in the open set.
                        if (!openSet.Contains(neighbourNode))
                            openSet.AddANode(neighbourNode);
                        else
                            openSet.UpdateNode(neighbourNode);
                    }
                }
            }

            return Retrace(endNode);
        }

        #endregion Class Methods
    }
}