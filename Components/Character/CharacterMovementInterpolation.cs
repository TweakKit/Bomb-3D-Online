using UnityEngine;

namespace ZB.Gameplay
{
    public class CharacterMovementInterpolation : MonoBehaviour
    {
        #region Members

        private static readonly float interpolationSpeed = 15.0f;
        private Transform _targetTransform;
        private Vector3 _currentPosition;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _targetTransform = transform.parent;
            _currentPosition = transform.position;
        }

        private void Update()
        {
            SmoothUpdate();
        }

        #endregion API Methods

        #region Class Methods

        private void SmoothUpdate()
        {
            _currentPosition = Vector3.Lerp(_currentPosition, _targetTransform.position, Time.deltaTime * interpolationSpeed);
            transform.position = _currentPosition;
        }

        #endregion Class Methods
    }
}