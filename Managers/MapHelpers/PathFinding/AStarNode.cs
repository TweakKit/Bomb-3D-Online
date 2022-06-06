    namespace ZB.Gameplay
{
    public class AStarNode : Node
    {
        #region Members

        private int _gCost;
        private int _hCost;

        #endregion Members

        #region Properties

        public int GCost { get => _gCost; set => _gCost = value; }
        public int HCost { get => _hCost; set => _hCost = value; }
        public int FCost { get => _gCost + _hCost; }

        #endregion Properties

        #region Class Methods

        public AStarNode(bool isTraversable, int gridIndexX, int gridIndexY) : base(isTraversable, gridIndexX, gridIndexY)
        {
            _gCost = 0;
            _hCost = 0;
        }

        public override void Reset()
        {
            base.Reset();
            _gCost = 0;
            _hCost = 0;
        }

        public override int CompareTo(object obj)
        {
            AStarNode otherNode = obj as AStarNode;

            int comparedResult = FCost.CompareTo(otherNode.FCost);

            if (comparedResult == 0)
                comparedResult = HCost.CompareTo(otherNode.HCost);

            return comparedResult;
        }

        #endregion Class Methods
    }
}