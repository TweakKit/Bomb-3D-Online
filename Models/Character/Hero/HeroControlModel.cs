using UnityEngine;
using ZB.Model;

namespace ZB.Gameplay
{
    public class HeroControlModel : HeroModel
    {
        #region Members

        private Vector3 _inputMoveDirection;

        #endregion Members

        #region Properties

        public override bool IsControlled { get { return true; } }

        public Vector3 InputMoveDirection
        {
            get
            {
                return _inputMoveDirection;
            }
            set
            {
                if (_inputMoveDirection != value)
                {
                    _inputMoveDirection = value;

                    if (isMoving && _inputMoveDirection == Vector3.zero)
                    {
                        isMoving = false;
                        ChangeMovementEvent.Invoke();
                    }
                    else if (!isMoving && _inputMoveDirection != Vector3.zero)
                    {
                        isMoving = true;
                        ChangeMovementEvent.Invoke();
                    }
                }
            }
        }

        #endregion Properties

        #region Class Methods

        public HeroControlModel(InventoryItem characterData) : base(characterData)
        {
            _inputMoveDirection = Vector3.zero;
        }

        public override void SetActivation(bool isActivated)
        {
            this.isActivated = isActivated;
            if (!isActivated)
                InputMoveDirection = Vector3.zero;
        }

        #endregion Class Methods
    }
}