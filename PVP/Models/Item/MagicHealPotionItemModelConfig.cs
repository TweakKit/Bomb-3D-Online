using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class MagicHealPotionItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private MagicHealPotionItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}