using UnityEngine;

namespace ZB.Gameplay
{
    public class BackgroundMusic : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private float _fadeDuration = 2.0f;
        [SerializeField]
        private float _volume = 0.5f;

        #endregion Members

        #region API Methods

        private void Start()
        {
            SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.backgroundMusicClips.GetRandomFromArray(), _volume, _fadeDuration);
        }

        #endregion API Methods
    }
}