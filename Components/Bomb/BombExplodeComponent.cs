using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BombInstanceComponent))]
    public class BombExplodeComponent : BombComponent
    {
        #region Members

        private static readonly float explodeDelay = 0.5f;
        private static readonly float explodeShakeSpeed = 12.0f;
        private static readonly float explodeShakeScaleBonus = 0.15f;
        private static readonly Vector3 originalScale = Vector3.one;

        private float _damageValue;
        private int _damageRadius;
        private bool _isOwnedByHero;
        private bool _hasExploded;
        private BoxCollider _collider;
        private Transform _visualTransform;
        private BombExplosionEffectType _bombExplosionEffectType;

        #endregion Members

        #region Properties

        public bool CanExplode { private get; set; }
        private Action ExplodeEvent { get; set; } = () => { };

        #endregion Properties

        #region API Methods

        private void Awake()
        {
            _collider = gameObject.GetComponent<BoxCollider>();
            _collider.enabled = false;
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(BombModel model)
        {
            base.InitModel(model);

            _visualTransform = model.visualTransform;
            _visualTransform.localScale = originalScale;
            _damageValue = model.DamageValue;
            _damageRadius = model.DamageRadius;
            _isOwnedByHero = model.IsOwnedByHero;
            _bombExplosionEffectType = model.bombType.GetBombExplosionEffectType();
            _hasExploded = false;
            _collider.enabled = true;

            CanExplode = true;
            ExplodeEvent = model.ExplodeEvent;
            StartCoroutine(Explode(model.Lifetime - explodeDelay));
        }

        private IEnumerator Explode(float delayTime)
        {
            float currentDelayTime = 0.0f;
            while (true)
            {
                if (CanExplode)
                {
                    currentDelayTime += Time.deltaTime;
                    if (currentDelayTime >= delayTime)
                        break;
                    yield return null;
                }
                else yield return null;
            }

            float currentExplodeDelay = 0.0f;
            float value = 0.0f;
            while (true)
            {
                if (CanExplode)
                {
                    currentExplodeDelay += Time.deltaTime;
                    if (currentExplodeDelay < explodeDelay)
                    {
                        value += Time.deltaTime * explodeShakeSpeed;
                        if (value > 1)
                            value = 0.0f;

                        _visualTransform.localScale = originalScale * (1 + (explodeShakeScaleBonus * 4 * (-value * value + value)));
                        yield return null;
                    }
                    else break;
                }
                else yield return null;
            }

            if (!_hasExploded)
            {
                _hasExploded = true;
                _collider.enabled = false;

                SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_bomb_explosion);
                MapManager.UpdateMap(transform.position, MapSlotType.Empty);

                DetectTargets();
                ExplodeEvent.Invoke();
                ExplodeEvent = null;

                PoolManager.ReturnObject(gameObject);
            }
        }

        private void PreExplodeByHit()
        {
            _collider.enabled = false;
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);
        }

        private void ExplodeByHit()
        {
            if (!_hasExploded)
            {
                _hasExploded = true;
                StopAllCoroutines();

                DetectTargets();
                ExplodeEvent.Invoke();
                ExplodeEvent = null;

                PoolManager.ReturnObject(gameObject);
            }
        }

        private void DetectTargets()
        {
            // Find damagable targets that are within the damage radius.
            var targets = Physics.OverlapSphere(transform.position, (_damageRadius + 1) * MapSetting.MapSquareSize, Constants.BombHitLayerMask);
            List<GameObject> neighbourBombs = null;

            // Find targets in horizontal direction.
            for (int x = 1; x <= _damageRadius; x++)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs))
                    break;

            for (int x = -1; x >= -_damageRadius; x--)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs))
                    break;

            // Find targets in vertical direction.
            for (int y = 1; y <= _damageRadius; y++)
                if (CheckDetection(transform.position + Vector3.forward * y * MapSetting.MapSquareSize, targets, ref neighbourBombs))
                    break;

            for (int y = -1; y >= -_damageRadius; y--)
                if (CheckDetection(transform.position + Vector3.forward * y * MapSetting.MapSquareSize, targets, ref neighbourBombs))
                    break;

            // Find targets in the center position of this bomb.
            DamageCharacters(transform.position, targets);

            // Explode neighbour bombs.
            if (neighbourBombs != null)
            {
                neighbourBombs.ForEach(x => x.GetComponent<BombExplodeComponent>().PreExplodeByHit());
                neighbourBombs.ForEach(x => x.GetComponent<BombExplodeComponent>().ExplodeByHit());
            }
        }

        private bool CheckDetection(Vector3 checkedPosition, Collider[] targets, ref List<GameObject> neighbourBombs)
        {
            if (MapManager.IsBreakablePosition(checkedPosition))
            {
                ApplyDamageToTarget(targets.First(x => x.transform.position == checkedPosition));
                return true;
            }
            else if (MapManager.IsBombPosition(checkedPosition))
            {
                if (neighbourBombs == null)
                    neighbourBombs = new List<GameObject>();

                neighbourBombs.Add(targets.First(x => x.transform.position == checkedPosition).gameObject);
                return true;
            }
            else if (MapManager.IsBlockPosition(checkedPosition))
            {
                return true;
            }
            else
            {
                DamageCharacters(checkedPosition, targets);
                return false;
            }
        }

        private void DamageCharacters(Vector3 checkedPosition, Collider[] targets)
        {
            foreach (var target in targets)
                if (MapManager.GetMapPosition(target.transform.position) == checkedPosition)
                    ApplyDamageToTarget(target);

            CreateExplosionEffect(checkedPosition);
        }

        private void CreateExplosionEffect(Vector3 explosionPosition)
        {
            GameObject explosionEffectGameObject = PoolManager.GetObject(_bombExplosionEffectType);
            explosionEffectGameObject.transform.position = explosionPosition + Vector3.up * MapSetting.EffectGroundOffset;
        }

        private void ApplyDamageToTarget(Collider target)
        {
            IEntityGetHit hitEntity = target.gameObject.GetComponent<IEntityGetHit>();
            hitEntity.GetHit(_damageValue, _isOwnedByHero ? HitBy.HeroBomb : HitBy.EnemyBomb);
        }

        #endregion Class Methods
    }
}