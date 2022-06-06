using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterControlMovementComponent : CharacterComponent
    {
        #region Members

        protected static readonly float rotateSpeed = 10.0f;
        protected HeroControlModel _heroControlModel;
        protected Rigidbody _rigidbody;

        #endregion Members

        #region API Methods

        protected virtual void Awake()
        {
            _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected virtual void FixedUpdate()
        {
            UpdateComponent();
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            if (model is HeroControlModel)
            {
                _heroControlModel = model as HeroControlModel;
                _heroControlModel.ChangeMovementEvent += OnChangeMovement;
            }
            else Destroy(this);
        }

        protected virtual void OnChangeMovement()
        {
            if (_model.isMoving)
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            else
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected virtual void UpdateComponent()
        {
            if (_heroControlModel.isActivated && !_heroControlModel.IsDead)
            {
                UpdateMovement();
                UpdateRotation();
            }
        }

        protected virtual void UpdateMovement()
        {
            if (CheckMoveable())
                _rigidbody.MovePosition(_rigidbody.position + _heroControlModel.InputMoveDirection * _heroControlModel.MoveSpeed * Time.deltaTime);
        }

        protected virtual void UpdateRotation()
        {
            if (_heroControlModel.rotateDirection != Vector3.zero)
                _model.visualTransform.rotation = Quaternion.Lerp(_heroControlModel.Rotation, Quaternion.LookRotation(_heroControlModel.rotateDirection), rotateSpeed * Time.deltaTime);
        }

        protected virtual bool CheckMoveable()
        {
            if (_heroControlModel.isMoving)
            {
                return !(MapManager.IsBombPosition(_rigidbody.position + (_heroControlModel.InputMoveDirection * _heroControlModel.MoveSpeed) * Time.deltaTime + _heroControlModel.InputMoveDirection * CharacterModel.CheckMoveableRadius) &&
                         MapManager.IsEmptyPosition(_rigidbody.position));
            }

            return false;
        }

        #endregion Class Methods
    }
}