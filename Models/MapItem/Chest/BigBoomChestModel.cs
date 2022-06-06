using ZB.Model;

namespace ZB.Gameplay
{
    public class BigBoomChestModel : MapItemModel
    {
        #region Members

        private float _trapRate;
        private int _trapDamage;
        private int _timeBurst;
        private int _trapArea;
        private float _tokenRate;
        private float _token;

        #endregion Members

        #region Properties

        public float TrapRate => _trapRate;
        public int TrapDamage => _trapDamage;
        public int TimeBurst => _timeBurst;
        public int TrapArea => _trapArea;
        public float TokenRate => _tokenRate;
        public float Token => _token;

        #endregion Properties

        #region Class Methods

        public BigBoomChestModel(BigBombChestStat bigBombChestStat, MapItemType mapItemType)
            : base(bigBombChestStat.name, bigBombChestStat.Info, mapItemType, bigBombChestStat.hp, 1)
        {
            _trapRate = bigBombChestStat.trapRate;
            _trapDamage = bigBombChestStat.trapDmg;
            _timeBurst = bigBombChestStat.trapTimeBurst;
            _trapArea = bigBombChestStat.trapArea;
            _tokenRate = bigBombChestStat.tokenRate;
            _token = bigBombChestStat.tokenDrop;
        }

        #endregion Class Methods
    }
}