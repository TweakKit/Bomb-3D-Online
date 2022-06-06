using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterControlInputComponent : CharacterComponent
    {
        #region Members

        protected static readonly string placeBombInputValue = "PlaceBomb";
        protected static readonly string horizontalInputValue = "Horizontal";
        protected static readonly string verticalInputValue = "Vertical";

        protected Vector3 _inputDirection;
        protected HeroControlModel _heroControlModel;

        #endregion Members

        #region API Methods
        
        private void Awake()
        {
            EventManager.AddListener(GameEventType.ClickPlaceBomb, OnClickPlaceBomb);
        }

        private void Update()
        {
            UpdateComponent();
        }

        #endregion API Methods

        #region Class Methods

        public void OnClickPlaceBomb()
        {
            if (_heroControlModel.CanAttack && MapManager.IsEmptyPosition(_heroControlModel.Position))
                _heroControlModel.AttackEvent.Invoke();
        }

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            if (model is HeroControlModel)
                _heroControlModel = model as HeroControlModel;
            else
                Destroy(this);
        }

        protected virtual void UpdateComponent()
        {
            if (_heroControlModel.isActivated && !_heroControlModel.IsDead)
            {
                if (Input.GetButtonDown(placeBombInputValue))
                    OnClickPlaceBomb();

#if UNITY_WEBGL || UNITY_EDITOR
                _inputDirection = new Vector3(Input.GetAxisRaw(horizontalInputValue), 0.0f, Input.GetAxisRaw(verticalInputValue));
#else
                _inputDirection = FixedJoystick.Instance.Direction;
#endif
                _inputDirection = _inputDirection.normalized;
                _heroControlModel.InputMoveDirection = _inputDirection;
                _heroControlModel.rotateDirection = _inputDirection;
            }
        }

        #endregion Class Methods
    }
}