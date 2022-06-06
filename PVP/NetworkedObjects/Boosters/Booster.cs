using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public abstract class Booster : NetworkedEntity
    {
        #region Members

        private static readonly float rotateAroundSpeed = 3.0f;
        [SerializeField]
        private GameObject _collectEffectPrefab;

        #endregion Members

        #region API Methods

        protected virtual void Update()
        {
            Animate();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (isDead)
                return;

            NetworkedPlayer networkedPlayer = collision.gameObject.GetComponent<NetworkedPlayer>();
            if (networkedPlayer)
            {
                isDead = true;
                if (isServer)
                {
                    networkedPlayer.CollectBoosterEvent.Invoke(this);
                    DestroySelf();
                }
                else CreateFX();
            }
        }

        #endregion API Methods

        #region Class Methods

        public virtual void GetCollected(NetworkedPlayer networkedPlayer) { }

        protected virtual void Animate()
        {
            transform.Rotate(Vector3.up * rotateAroundSpeed);
        }

        protected virtual void CreateFX()
        {
            GameObject collectEffect = Instantiate(_collectEffectPrefab);
            collectEffect.transform.position = transform.position + Vector3.up * MapSetting.EffectGroundOffset;
        }

        protected virtual void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion Class Methods
    }
}