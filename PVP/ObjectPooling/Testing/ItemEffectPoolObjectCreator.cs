using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class ItemEffectPoolObjectCreator : PoolObjectCreator
    {
        #region Members

        [SerializeField]
        private ItemEffectType _itemEffectType;

        #endregion Members

        #region Properties

        public override Enum ObjectType => _itemEffectType;

        #endregion Properties
    }

    public enum ItemEffectType
    {
        RocketLaunchTileExplosionEffect = 1,
    }
}