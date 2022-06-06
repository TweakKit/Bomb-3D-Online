using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Members

        protected static readonly string placeBombInputValue = "PlaceBomb";
        protected static readonly string horizontalInputValue = "Horizontal";
        protected static readonly string verticalInputValue = "Vertical";
        protected Vector3 _inputDirection;

        #endregion Members

        #region Properties

        protected Vector3 InputDirection
        {
            get
            {
                return _inputDirection;
            }
            set
            {
                if (_inputDirection != value)
                {
                    _inputDirection = value;

                    if (isMoving && _inputDirection == Vector3.zero)
                    {
                        isMoving = false;
                        ChangeMovementEvent.Invoke();
                    }
                    else if (!isMoving && _inputDirection != Vector3.zero)
                    {
                        isMoving = true;
                        ChangeMovementEvent.Invoke();
                    }
                }
            }
        }

        #endregion Properties

        #region Class Methods

        public virtual void RegisterInput()
        {
            _inputDirection = Vector3.zero;
        }

        public virtual void UpdateInput()
        {
            if (isDead)
                return;

            if (Input.GetButtonDown(placeBombInputValue))
                ClickAttackEvent.Invoke();

            InputDirection = new Vector3(Input.GetAxisRaw(horizontalInputValue), 0.0f, Input.GetAxisRaw(verticalInputValue));
            InputDirection = InputDirection.normalized;
        }

        #endregion Class Methods
    }
}