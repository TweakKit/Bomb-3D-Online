using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Members

        protected static readonly float rotateSpeed = 10.0f;
        protected Rigidbody _rigidbody;

        #endregion Members

        #region Class Methods

        public virtual void RegisterMovement()
        {
            _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            ChangeMovementEvent += OnChangeMovement_CallbackOnMovement;
        }

        public virtual void UpdateMovement()
        {
            if (isDead)
                return;

            if (isMoving)
            {
                _rigidbody.MovePosition(_rigidbody.position + InputDirection * MoveSpeed * Time.deltaTime);
                _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(InputDirection), rotateSpeed * Time.deltaTime);
            }
        }

        protected virtual void OnChangeMovement_CallbackOnMovement()
        {
            if (isMoving)
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            else
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        #endregion Class Methods
    }
}