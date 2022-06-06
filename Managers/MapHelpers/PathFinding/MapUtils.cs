using System;
using System.Collections.Generic;

namespace ZB.Gameplay
{
    public static class MapUtils
    {
        #region Members

        private static readonly int normalEdgeDistance = 10;
        private static readonly int diagonalEdgeDistance = 14;

        #endregion Members

        #region Class Methods

        public static int GetHeuristicDistanceBetweenTwoNodes(Node firstNode, Node secondNode)
        {
            int deltaX = Math.Abs(secondNode.GridIndexX - firstNode.GridIndexX);
            int deltaY = Math.Abs(secondNode.GridIndexY - firstNode.GridIndexY);
            return Math.Min(deltaX, deltaY) * diagonalEdgeDistance + Math.Abs(deltaY - deltaX) * normalEdgeDistance;
        }

        public static int GetEuclideanDistanceNoSQRTBetweenTwoNodes(Node firstNode, Node secondNode)
        {
            return GetHeuristicDistanceBetweenTwoNodes(firstNode, secondNode);
        }

        public static List<Node> GetNeighbourNodes(Node[,] nodes, Node currentNode)
        {
            List<Node> neighbourNodes = new List<Node>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((i == 0 || j == 0) && (i ^ j) != 0)
                    {
                        int gridIndexX = currentNode.GridIndexX + i;
                        int gridIndexY = currentNode.GridIndexY + j;

                        if ((gridIndexX >= 0 && gridIndexX < nodes.GetLength(1)) && (gridIndexY >= 0 && gridIndexY < nodes.GetLength(0)))
                            neighbourNodes.Add(nodes[gridIndexY, gridIndexX]);
                    }
                }
            }

            return neighbourNodes;
        }

        #endregion Class Methods
    }
}
