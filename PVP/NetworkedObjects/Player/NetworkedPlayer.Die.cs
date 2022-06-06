using System.Collections;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Class Methods

        protected static readonly float throwSpeed = 10.0f;

        #endregion Class Methods

        #region Class Methods

        public virtual void RegisterDie()
        {
            DieEvent += OnDie_CallbackOnDie;
        }

        protected virtual void OnDie_CallbackOnDie()
        {
            CustomNetworkManager.singleton.StopClient();
        }

        [ClientRpc]
        protected virtual void RpcDie()
        {
            DestroyShadow();
            StartCoroutine(ThrowInAir());
        }

        protected virtual IEnumerator ThrowInAir()
        {
            Vector3 startPosition = transform.position;
            Vector3 destinationPosition = Vector3.zero;
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
                transform.forward = (transform.position - nextTravelPosition).normalized;
                transform.position = nextTravelPosition;

                yield return null;
            }

            transform.forward = (transform.position - destinationPosition).normalized;
            transform.position = destinationPosition;
        }

        protected virtual void DestroyShadow()
        {
            GameObject shadowGameObject = gameObject.GetChild(Constants.CharacterShadowName);
            Destroy(shadowGameObject);
        }

        #endregion Class Methods
    }
}