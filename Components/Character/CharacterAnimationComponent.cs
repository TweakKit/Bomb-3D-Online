using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterAnimationComponent : CharacterComponent
    {
        #region Members

        protected static readonly int moveSpeedAnimationParameter = Animator.StringToHash("MoveSpeed");
        protected static readonly int isDeadAnimationParameter = Animator.StringToHash("IsDead");
        protected static readonly int isWinAnimationParameter = Animator.StringToHash("IsWin");
        protected static readonly int isLoseAnimationParameter = Animator.StringToHash("IsLose");
        protected Animator _animator;

        #endregion Members

        #region API Methods

        protected virtual void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            _model.ChangeMovementEvent += OnChangeMovement;
            _model.WinEvent += OnWin;
            _model.LoseEvent += OnLose;
            _model.DieEvent += OnDie;
        }

        protected virtual void OnChangeMovement()
        {
            if (_model.isMoving)
                _animator.SetFloat(moveSpeedAnimationParameter, _model.MoveSpeed);
            else
                _animator.SetFloat(moveSpeedAnimationParameter, 0.0f);
        }

        protected virtual void OnWin()
        {
            _animator.SetTrigger(isWinAnimationParameter);
        }

        protected virtual void OnLose()
        {
            _animator.SetTrigger(isLoseAnimationParameter);
        }

        protected virtual void OnDie()
        {
            _animator.SetTrigger(isDeadAnimationParameter);
        }

        #endregion Class Methods
    }
}