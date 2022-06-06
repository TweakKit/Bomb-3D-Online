using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class SelfDefendItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private SelfDefendItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}