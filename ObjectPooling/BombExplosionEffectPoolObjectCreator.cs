using System;
using UnityEngine;

namespace ZB.Gameplay
{
    public class BombExplosionEffectPoolObjectCreator : PoolObjectCreator
    {
        #region Members

        [SerializeField]
        private BombExplosionEffectType _bombExplosionEffectType;

        #endregion Members

        #region Properties

        public override Enum ObjectType => _bombExplosionEffectType;

        #endregion Properties
    }

    [Serializable]
    public enum BombExplosionEffectType
    {
        BombExplosionEffectEarth = 1,
        BombExplosionEffectFire = 2,
        BombExplosionEffectWater = 3,
        BombExplosionEffectAir = 4,
        BombExplosionEffectDefault = 5,
    }
}