using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkedMapItem : NetworkedEntity, IEntityGetHit
    {
        #region Members

        [SerializeField]
        private MapItemType _mapItemType;
        [SerializeField]
        private GameObject _explosionEffectPrefab;

        #endregion Members

        #region Class Methods

        [Server]
        public virtual void GetHit(float damageValue)
        {
            if (!isDead)
            {
                isDead = true;
                RpcCreateFX(transform.position);
                DestroySelf();
            }
        }

        [Server]
        protected virtual void DestroySelf()
        {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
            MapManager.UpdateMap(transform.position, MapSlotType.Empty);

            if (_mapItemType == MapItemType.BrickBox)
                EventManager.Invoke<Vector3>(GameEventType.ExposeHiddenItem, transform.position);
        }

        [ClientRpc]
        protected virtual void RpcCreateFX(Vector3 explosionPosition)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.SFX_ingame_brick_explosion);
            GameObject explosionEffect = NetworkPoolManager.Spawn(_explosionEffectPrefab);
            explosionEffect.transform.position = transform.position + Vector3.up * MapSetting.EffectGroundOffset;
        }

        #endregion Class Methods
    }
}