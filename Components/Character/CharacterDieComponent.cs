using System.Collections;
using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterDieComponent : CharacterComponent
    {
        #region Members

        protected static readonly float throwSpeed = 10.0f;

        #endregion Members

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            if (_model.IsHero)
                _model.DieEvent += OnHeroDie;
            else
                _model.DieEvent += OnEnemyDie;
        }

        protected virtual void OnHeroDie()
        {
            EventManager.Invoke<CharacterModel>(GameEventType.HeroDie, _model);
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_hero_die);
            DestroyShadow();
            StartCoroutine(ThrowInAir());
        }

        protected virtual void OnEnemyDie()
        {
            EventManager.Invoke<CharacterModel>(GameEventType.EnemyDie, _model);
            DestroyShadow();
            StartCoroutine(ThrowInAir());
        }

        protected virtual IEnumerator ThrowInAir()
        {
            Vector3 startPosition = transform.position;
            Vector3 destinationPosition = MapManager.GetOffBorderPosition(transform.position);
            Vector3 deltaPosition = destinationPosition - startPosition;
            float throwHeight = deltaPosition.magnitude / 4;
            float throwTime = Mathf.Abs(deltaPosition.magnitude / throwSpeed);
            float currentThrowTime = 0.0f;

            while (currentThrowTime < throwTime)
            {
                currentThrowTime += Time.deltaTime;
                float timeRatio = currentThrowTime / throwTime;
                Vector3 nextTravelPosition = startPosition;
                nextTravelPosition.y += deltaPosition.y * timeRatio + Mathf.Sin(timeRatio * Mathf.PI) * throwHeight;
                nextTravelPosition.x += deltaPosition.x * timeRatio;
                nextTravelPosition.z += deltaPosition.z * timeRatio;
                _model.visualTransform.forward = (transform.position - nextTravelPosition).normalized;
                transform.position = nextTravelPosition;

                yield return null;
            }

            _model.visualTransform.forward = (transform.position - destinationPosition).normalized;
            transform.position = destinationPosition;

            yield return null;

            DestroySelf();
        }

        protected virtual void DestroyShadow()
        {
            GameObject shadowGameObject = gameObject.GetChild(Constants.CharacterShadowName);
            Destroy(shadowGameObject);
        }

        protected virtual void DestroySelf()
        {
            Destroy(gameObject);
        }

        #endregion Class Methods
    }
}