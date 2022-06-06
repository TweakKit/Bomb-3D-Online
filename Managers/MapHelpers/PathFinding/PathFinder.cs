using System.Collections.Generic;

namespace ZB.Gameplay
{
    public abstract class PathFinder
    {
        #region Class Methods

        public abstract List<Node> Find(Node[,] nodes, Node startNode, Node endNode);

        protected static List<Node> Retrace(Node endNode)
        {
            List<Node> shortestPathNodes = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                shortestPathNodes.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }
            
            shortestPathNodes.Reverse();
            return shortestPathNodes;
        }

        #endregion Class Methods
    }
}