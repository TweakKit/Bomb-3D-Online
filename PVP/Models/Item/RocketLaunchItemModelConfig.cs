using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class RocketLaunchItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private RocketLaunchItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}