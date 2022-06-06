using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class BoosterEffectPoolObjectCreator : PoolObjectCreator
    {
        #region Members

        [SerializeField]
        private BoosterEffectType _boosterEffectType;

        #endregion Members

        #region Properties

        public override Enum ObjectType => _boosterEffectType;

        #endregion Properties
    }

    public enum BoosterEffectType
    {
        PowerUpBoosterCollectEffect = 1,
        SpeedUpBoosterCollectEffect = 2,
        RangeUpBoosterCollectEffect = 3,
        BombUpBoosterCollectEffect = 4,
        SpeedBoostBoosterCollectEffect = 5,
        BombBoostBoosterCollectEffect = 6,
        MagnetBoosterCollectEffect = 7,
    }
}