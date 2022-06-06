using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class SlowTrapItemModelConfig : ItemModelConfig
    {
        #region Members

        [SerializeField]
        private SlowTrapItemModel _model;

        #endregion Members

        #region Properties

        protected override ItemModel ItemModel => _model;

        #endregion Properties
    }
}