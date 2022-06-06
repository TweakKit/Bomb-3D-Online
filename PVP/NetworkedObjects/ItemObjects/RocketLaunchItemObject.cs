using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class RocketLaunchItemObject : NetworkedEntity
    {
        #region Membes

        [SerializeField]
        private float _startFlyingDelay = 3.0f;
        [SerializeField]
        private float _flySpeed = 10.0f;
        [SerializeField]
        private float _explosionRadius = 3;
        [SerializeField]
        private float _hitDamageValue = 50.0f;
        [SerializeField]
        private float _explodeDamageValue = 100.0f;
        [SerializeField]
        private GameObject _tileExplosionEffectPrefab;
        private Rigidbody _rigidbody;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isDead)
                return;

            isDead = true;

            if (isServer)
            {
                DetectTargets();
                DestroySelf();
            }
        }

        #endregion API Methods

        #region Class Methods

        public override void OnStartServer()
        {
            StartCoroutine(Fly());
        }

        public override void OnStartClient()
        {
            StartCoroutine(Fly());
        }

        private IEnumerator Fly()
        {
            yield return new WaitForSeconds(_startFlyingDelay);
            _rigidbody.velocity = _flySpeed * transform.forward;
        }

        [Server]
        private void DetectTargets()
        {
            var targets = Physics.OverlapSphere(transform.position, (_explosionRadius + 1) * MapSetting.MapSquareSize, Constants.BombHitLayerMask);
            List<GameObject> neighbourBombs = null;
            List<Vector3> explosionPositions = new List<Vector3>() { transform.position };

            for (int x = 1; x <= _explosionRadius; x++)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int x = -1; x >= -_explosionRadius; x--)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int y = 1; y <= _explosionRadius; y++)
                if (CheckDetection(transform.position + Vector3.forward * y * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int y = -1; y >= -_explosionRadius; y--)
                if (CheckDetection(transform.position + Vector3.forward * y * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            DamageCharacters(transform.position, targets);
            RpcCreateFX(explosionPositions);

            if (neighbourBombs != null)
            {
                neighbourBombs.ForEach(x => x.GetComponent<NetworkedBomb>().PreExplodeByHit());
                neighbourBombs.ForEach(x => x.GetComponent<NetworkedBomb>().ExplodeByHit());
            }
        }

        [Server]
        private bool CheckDetection(Vector3 checkedPosition, Collider[] targets, ref List<GameObject> neighbourBombs, ref List<Vector3> explosionPositions)
        {
            if (MapManager.IsBreakablePosition(checkedPosition))
            {
                var target = targets.FirstOrDefault(x => x.transform.position == checkedPosition);
                if (target)
                    ApplyDamageToTarget(target);

                return false;
            }
            else if (MapManager.IsBombPosition(checkedPosition))
            {
                if (neighbourBombs == null)
                    neighbourBombs = new List<GameObject>();

                var target = targets.FirstOrDefault(x => x.transform.position == checkedPosition);
                if (target)
                    neighbourBombs.Add(target.gameObject);

                return false;
            }
            else if (MapManager.IsBlockPosition(checkedPosition))
            {
                return true;
            }
            else
            {
                DamageCharacters(checkedPosition, targets);

                if (explosionPositions == null)
                    explosionPositions = new List<Vector3>();
                explosionPositions.Add(checkedPosition);

                return false;
            }
        }

        [Server]
        private void DamageCharacters(Vector3 checkedPosition, Collider[] targets)
        {
            foreach (var target in targets)
                if (MapManager.GetMapPosition(target.transform.position) == checkedPosition)
                    ApplyDamageToTarget(target);
        }

        [Server]
        private void ApplyDamageToTarget(Collider target)
        {
            IEntityGetHit hitEntity = target.gameObject.GetComponent<IEntityGetHit>();
            if (hitEntity != null)
                hitEntity.GetHit(_explodeDamageValue);
        }

        [ClientRpc]
        private void RpcCreateFX(List<Vector3> explosionPositions)
        {
            foreach (var explosionPosition in explosionPositions)
            {
                GameObject explosionEffectGameObject = Instantiate(_tileExplosionEffectPrefab);
                explosionEffectGameObject.transform.position = explosionPosition + Vector3.up * MapSetting.EffectGroundOffset;
            }
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion Class Methods
    }
}