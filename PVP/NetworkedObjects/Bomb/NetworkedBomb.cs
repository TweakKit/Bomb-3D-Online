using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkedBomb : NetworkBehaviour
    {
        #region Members

        private static readonly float explodeDelay = 0.5f;
        private static readonly float explodeShakeSpeed = 12.0f;
        private static readonly float explodeShakeScaleBonus = 0.15f;
        private static readonly Vector3 originalScale = Vector3.one;

        [HideInInspector]
        [SyncVar]
        public uint ownerNetID;

        [HideInInspector]
        [SyncVar]
        public NetworkedBombData bombData;

        [SerializeField]
        private GameObject _explosionEffectPrefab;
        private bool _hasExploded;
        private Collider _collider;
        private List<Collider> _ignoredColliders;
        private Transform _visualTransform;

        #endregion Members

        #region Properties

        public NetworkedPlayer Owner
        {
            get
            {
                var player = NetworkServer.active ? NetworkServer.spawned[ownerNetID] : NetworkClient.spawned[ownerNetID];
                return player.GetComponent<NetworkedPlayer>();
            }
        }

        #endregion Properties

        #region API Methods

        private void Awake()
        {
            _collider = gameObject.GetComponent<Collider>();
            _collider.isTrigger = true;
            _visualTransform = transform.FindChildByName(Constants.EntityVisualName);
        }

        private void Start()
        {
            CheckPenetration();
        }

        private void FixedUpdate()
        {
            if (_hasExploded)
                return;

            UpdatePenetration();
        }

        #endregion API Methods

        #region Class Methods

        public void Init(uint ownerNetID, NetworkedBombData bombData)
        {
            this.ownerNetID = ownerNetID;
            this.bombData = bombData;
        }

        public override void OnStartServer()
        {
            _hasExploded = false;
            _collider.enabled = true;
            MapManager.UpdateMap(transform.position, MapSlotType.Bomb);
            StartCoroutine(Explode(bombData.ExplosionTime - explodeDelay));
        }

        public override void OnStartClient()
        {
            _hasExploded = false;
            _collider.enabled = true;
            StartCoroutine(Explode(bombData.ExplosionTime - explodeDelay));
        }

        private IEnumerator Explode(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            float value = 0.0f;
            float currentExplodeDelay = 0.0f;
            while (currentExplodeDelay < explodeDelay)
            {
                currentExplodeDelay += Time.deltaTime;
                value += Time.deltaTime * explodeShakeSpeed;

                if (value > 1)
                    value = 0.0f;

                _visualTransform.localScale = originalScale * (1 + (explodeShakeScaleBonus * 4 * (-value * value + value)));
                yield return null;
            }

            if (_hasExploded)
                yield break;

            _hasExploded = true;
            _collider.enabled = false;
            ResetPenetration();

            if (!isServer)
                yield break;

            DetectTargets();
            DestroySelf();
        }

        [Server]
        public void PreExplodeByHit()
        {
            _collider.enabled = false;
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);
        }

        private void CheckPenetration()
        {
            _visualTransform.localScale = originalScale;
            _ignoredColliders = new List<Collider>();

            var collidedObjects = Physics.OverlapSphere(transform.position, MapSetting.MapSquareHalfSize);
            foreach (var collidedObject in collidedObjects)
            {
                var player = collidedObject.GetComponent<NetworkedPlayer>();
                if (player != null)
                {
                    Physics.IgnoreCollision(player.collider, _collider, true);
                    _ignoredColliders.Add(player.collider);
                }
            }
            _collider.isTrigger = false;
        }

        private void UpdatePenetration()
        {
            foreach (var ignoredCollider in _ignoredColliders)
                if (Mathf.Abs(ignoredCollider.transform.position.x - transform.position.x) >= MapSetting.MapSquareSize || Mathf.Abs(ignoredCollider.transform.position.z - transform.position.z) >= MapSetting.MapSquareSize)
                    Physics.IgnoreCollision(ignoredCollider, _collider, false);
        }

        private void ResetPenetration()
        {
            foreach (var ignoredCollider in _ignoredColliders)
                Physics.IgnoreCollision(ignoredCollider, _collider, false);
        }

        [Server]
        public void ExplodeByHit()
        {
            if (!_hasExploded)
            {
                _hasExploded = true;
                StopAllCoroutines();

                DetectTargets();
                DestroySelf();
            }
        }

        [Server]
        private void DetectTargets()
        {
            var targets = Physics.OverlapSphere(transform.position, (bombData.DamageRadius + 1) * MapSetting.MapSquareSize, Constants.NetworkedBombHitLayerMask);
            List<GameObject> neighbourBombs = null;
            List<Vector3> explosionPositions = new List<Vector3>() { transform.position };

            for (int x = 1; x <= bombData.DamageRadius; x++)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int x = -1; x >= -bombData.DamageRadius; x--)
                if (CheckDetection(transform.position + Vector3.right * x * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int y = 1; y <= bombData.DamageRadius; y++)
                if (CheckDetection(transform.position + Vector3.forward * y * MapSetting.MapSquareSize, targets, ref neighbourBombs, ref explosionPositions))
                    break;

            for (int y = -1; y >= -bombData.DamageRadius; y--)
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

                return true;
            }
            else if (MapManager.IsBombPosition(checkedPosition))
            {
                if (neighbourBombs == null)
                    neighbourBombs = new List<GameObject>();

                var target = targets.FirstOrDefault(x => x.transform.position == checkedPosition);
                if (target)
                    neighbourBombs.Add(target.gameObject);

                return true;
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
                hitEntity.GetHit(bombData.DamageValue);
        }

        [Server]
        private void DestroySelf()
        {
            Owner.BombExploded();
            bombData = null;
            NetworkPoolManager.Despawn(gameObject);
            NetworkServer.UnSpawn(gameObject);
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);
        }

        [ClientRpc]
        private void RpcCreateFX(List<Vector3> explosionPositions)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_bomb_explosion);
            foreach (var explosionPosition in explosionPositions)
            {
                GameObject explosionEffectGameObject = NetworkPoolManager.Spawn(_explosionEffectPrefab);
                explosionEffectGameObject.transform.position = explosionPosition + Vector3.up * MapSetting.EffectGroundOffset;
            }
        }

        #endregion Class Methods
    }

    [Serializable]
    public class NetworkedBombData
    {
        #region Members

        [SerializeField]
        private BombType _bombType;
        [SerializeField]
        private float _explosionTime;
        [SerializeField]
        private float _damageValue;
        [SerializeField]
        private int _damageRadius;
        [NonSerialized]
        public float bonusDamageValue;
        [NonSerialized]
        public int bonusDamageRadius;

        #endregion Members

        #region Properties

        public BombType BombType => _bombType;
        public float ExplosionTime => _explosionTime;
        public float DamageValue => _damageValue + bonusDamageValue;
        public float DamageRadius => _damageRadius + bonusDamageRadius;

        #endregion Properties

        #region Class Methods

        public NetworkedBombData() { }

        public NetworkedBombData(BombType bombType, float explosionTime, float damageValue, int damageRadius)
        {
            _bombType = bombType;
            _explosionTime = explosionTime;
            _damageValue = damageValue;
            _damageRadius = damageRadius;
        }

        #endregion Class Methods
    }
}