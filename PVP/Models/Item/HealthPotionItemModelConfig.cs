using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class HealthPotionItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private HealthPotionItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}