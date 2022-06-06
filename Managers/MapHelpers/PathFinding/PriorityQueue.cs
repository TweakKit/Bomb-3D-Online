namespace ZB.Gameplay
{
    public class PriorityQueue
    {
        #region Members

        private Node[] _nodes;
        private int _currentNodeIndex;

        #endregion Members

        #region Classs Methods

        public PriorityQueue(int capacity)
        {
            _nodes = new Node[capacity];
            _currentNodeIndex = 0;
        }

        public void AddANode(Node node)
        {
            node.Index = _currentNodeIndex;
            _nodes[_currentNodeIndex++] = node;

            ShiftUp(node);
        }

        public Node GetMinimumCostNode()
        {
            Node minimumCostNode = _nodes[0];

            _nodes[0] = _nodes[--_currentNodeIndex];
            _nodes[0].Index = 0;

            ShiftDown(_nodes[0]);

            return minimumCostNode;
        }

        public void UpdateNode(Node node) => ShiftUp(node);
        public bool Contains(Node node) => Equals(_nodes[node.Index], node);
        public bool IsEmpty() => _currentNodeIndex == 0;

        private void ShiftUp(Node node)
        {
            int parentIndex = (node.Index - 1) / 2;

            while (true)
            {
                Node parentNode = _nodes[parentIndex];

                if (node.CompareTo(parentNode) == -1)
                    SwapNodes(node, parentNode);
                else
                    break;

                parentIndex = (node.Index - 1) / 2;
            }
        }

        private void ShiftDown(Node node)
        {
            int leftChildIndex;
            int rightChildIndex;
            int swappingIndex;

            while (true)
            {
                leftChildIndex = node.Index * 2 + 1;
                rightChildIndex = node.Index * 2 + 2;

                if (leftChildIndex < _currentNodeIndex)
                {
                    swappingIndex = leftChildIndex;

                    if (rightChildIndex < _currentNodeIndex)
                        if (_nodes[leftChildIndex].CompareTo(_nodes[rightChildIndex]) == 1)
                            swappingIndex = rightChildIndex;

                    if (node.CompareTo(_nodes[swappingIndex]) == 1)
                        SwapNodes(node, _nodes[swappingIndex]);
                    else
                        break;
                }
                else break;
            }
        }

        private void SwapNodes(Node firstNode, Node secondNode)
        {
            _nodes[firstNode.Index] = secondNode;
            _nodes[secondNode.Index] = firstNode;

            int tempIndex = firstNode.Index;
            firstNode.Index = secondNode.Index;
            secondNode.Index = tempIndex;
        }

        #endregion Class Methods
    }
}