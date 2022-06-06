using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class FlyBootsItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private FlyBootsItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}