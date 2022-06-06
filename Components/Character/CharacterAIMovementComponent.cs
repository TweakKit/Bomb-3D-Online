using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterAIMovementComponent : CharacterComponent
    {
        #region API Methods

        protected virtual void Update()
        {
            UpdateComponent();
        }

        #endregion API Methods

        #region Class Methods

        protected virtual void UpdateComponent()
        {
            if (_model.isActivated && !_model.IsDead)
            {
                UpdateMovement();
                UpdateRotation();
            }
        }

        protected virtual void UpdateMovement()
        {
            if (_model.isMoving)
                transform.position = _model.MovePosition;
        }

        protected virtual void UpdateRotation()
        {
            if (_model.rotateDirection != Vector3.zero)
                _model.visualTransform.forward = _model.rotateDirection;
        }

        #endregion Class Methods
    }
}