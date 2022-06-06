using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemGetHitComponent : MapItemComponent, IEntityGetHit
    {
        #region Class Methods

        public virtual void GetHit(float damageValue, HitBy hitBy)
        {
            if (!_model.IsDead)
            {
                _model.hitBy = hitBy;
                _model.currentHP -= damageValue;
                _model.GetDamagedEvent.Invoke();

                if (_model.currentHP <= 0)
                    _model.DieEvent.Invoke();
            }
        }

        #endregion Class Methods
    }
}