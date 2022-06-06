using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public abstract class TimeLimitBooster : Booster
    {
        #region Membes

        [SerializeField]
        protected float _usageLimitTime = 1.0f;

        #endregion Members
    }
}