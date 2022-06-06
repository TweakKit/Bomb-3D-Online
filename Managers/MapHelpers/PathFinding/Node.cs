using System;

namespace ZB.Gameplay
{
    public abstract class Node : IComparable, IPriorityQueueItem
    {
        #region Members

        private int _index;
        private bool _isTraversable;
        private int _gridIndexX;
        private int _gridIndexY;
        private Node _parentNode;

        #endregion Members

        #region Properties

        public int Index { get => _index; set => _index = value; }
        public bool IsTraversable { get => _isTraversable; set => _isTraversable = value; }
        public int GridIndexX { get => _gridIndexX; set => _gridIndexX = value; }
        public int GridIndexY { get => _gridIndexY; set => _gridIndexY = value; }
        public Node ParentNode { get => _parentNode; set => _parentNode = value; }

        #endregion Properties

        #region Class Methods

        public Node(bool isTraversable, int gridIndexX, int gridIndexY)
        {
            _isTraversable = isTraversable;
            _gridIndexX = gridIndexX;
            _gridIndexY = gridIndexY;
            _index = 0;
            _parentNode = null;
        }

        public virtual void Reset()
        {
            _index = 0;
            _parentNode = null;
        }

        public abstract int CompareTo(object obj);

        #endregion Class Methods
    }
}