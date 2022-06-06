using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class NoBlockItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private NoBlockItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}