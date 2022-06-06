using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class NetworkCameraManager : MonoBehaviour
    {
        #region Members

        [SerializeField]
        [Tooltip("The clamped distance in the x-z plane to the target.")]
        private float _distance = 10.0f;

        [SerializeField]
        [Tooltip("The clamped height the camera should be above the target.")]
        private float _height = 5.0f;

        private Transform _target;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            EventManager.AddListener<NetworkedPlayer>(GameEventType.InitPlayer, OnSpawnPlayer);
        }

        private void Start()
        {
            Transform listenerTransform = gameObject.GetComponentInChildren<AudioListener>().transform;
            listenerTransform.position = transform.position + transform.forward * _distance;
        }

        private void LateUpdate()
        {
            if (_target)
                transform.position = _target.position - transform.forward * _distance + Vector3.up * _height;
        }

        #endregion API Methods

        #region Class Methods

        private void OnSpawnPlayer(NetworkedPlayer player)
        {
            _target = player.transform;
        }

        #endregion Class Methods
    }
}