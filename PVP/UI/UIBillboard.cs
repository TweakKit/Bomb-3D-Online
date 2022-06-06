using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class UIBillboard : MonoBehaviour
    {
        #region Members

        [SerializeField]
        [Tooltip("If enabled, scale this object to always stay at the same size, regardless of the position in the scene, i.e. distance to camera.")]
        private bool _scaleWithDistance = false;

        [SerializeField]
        [Tooltip("Multiplier applied to the distance scale calculation.")]
        private float _scaleMultiplier = 1f;

        private Transform _cameraTransform;
        private float _size;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);

            if (!_scaleWithDistance)
                return;

            _size = (_cameraTransform.position - transform.position).magnitude;
            transform.localScale = Vector3.one * (_size * (_scaleMultiplier / 100f));
        }

        #endregion API Methods
    }
}