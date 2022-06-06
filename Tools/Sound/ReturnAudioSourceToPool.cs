using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    public class ReturnAudioSourceToPool : MonoBehaviour
    {
        #region Members

        private AudioSource _audioSource;
        private float _destroySourceDelayTime;

        #endregion Members

        #region API Methods

        public void OnEnable()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            _destroySourceDelayTime = _audioSource.clip.length;
            Invoke("InvokeReturnToPool", _destroySourceDelayTime);
        }

        #endregion API Methods

        #region Class Methods

        private void InvokeReturnToPool()
        {
            SoundManager.Instance.ReturnAudioSourceToPool(_audioSource);
        }

        #endregion Class Methods
    }
}