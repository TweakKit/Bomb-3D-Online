using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class PoolParticle : MonoBehaviour
    {
        #region API Methods

        private void OnParticleSystemStopped()
        {
            NetworkPoolManager.Despawn(gameObject);
        }

        #endregion API Methods
    }
}