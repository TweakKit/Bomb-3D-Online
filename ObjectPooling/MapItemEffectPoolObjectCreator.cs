using System;
using UnityEngine;

namespace ZB.Gameplay
{
    public class MapItemEffectPoolObjectCreator : PoolObjectCreator
    {
        #region Members

        [SerializeField]
        private MapItemEffectType _mapItemEffectType;

        #endregion Members

        #region Properties

        public override Enum ObjectType => _mapItemEffectType;

        #endregion Properties
    }

    public enum MapItemEffectType
    {
        BrickDecorTileExplosionEffect = 1,
        BrickBoxTileExplosionEffect = 2,
        ChestBoxTileExplosionEffect = 3,
    }
}