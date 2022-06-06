using System.Collections;
using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemShakeGetHitComponent : MapItemGetHitComponent
    {
        #region Members

        private static readonly int shakeTimes = 2;
        private static readonly float shakeSpeed = 10.0f;
        private static readonly float shakeScaleBonus = 0.1f;
        private Vector3 _originalScale;

        #endregion Members

        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);
            _originalScale = _model.visualTransform.localScale;
        }

        public override void GetHit(float damageValue, HitBy hitBy)
        {
            base.GetHit(damageValue, hitBy);

            if (!_model.IsDead)
            {
                StopCoroutine("Shake");
                StartCoroutine("Shake");
            }
        }

        private IEnumerator Shake()
        {
            float time = 0.0f;
            int currentShakeTimes = 1;

            while (currentShakeTimes <= shakeTimes)
            {
                while (time < 1.0f)
                {
                    time += Time.deltaTime * shakeSpeed;
                    _model.visualTransform.localScale = _originalScale * (1 + (shakeScaleBonus * 4 * (-time * time + time)));
                    yield return null;
                }

                _model.visualTransform.localScale = _originalScale;
                time = 0.0f;
                currentShakeTimes++;
            }
        }

        #endregion Class Methods
    }
}