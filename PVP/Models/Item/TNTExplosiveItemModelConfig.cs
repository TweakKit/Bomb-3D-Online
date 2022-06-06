using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class TNTExplosiveItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private TNTExplosiveItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}