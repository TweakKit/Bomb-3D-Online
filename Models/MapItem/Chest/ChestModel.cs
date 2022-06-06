using ZB.Model;

namespace ZB.Gameplay
{
    public class ChestModel : MapItemModel
    {
        #region Members

        private float _token;

        #endregion Members

        #region Properties

        public float Token => _token;

        #endregion Properties

        #region Class Methods

        public ChestModel(ChestStat chestStat, MapItemType mapItemType, float baseToken)
            : base(chestStat.name, chestStat.Info, mapItemType, chestStat.hp, chestStat.level)
        {
            _token = chestStat.rate * baseToken;
        }

        #endregion Class Methods
    }
}