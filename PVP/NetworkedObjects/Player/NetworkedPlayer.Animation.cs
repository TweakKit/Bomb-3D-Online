using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Class Methods

        protected static readonly int moveSpeedAnimationParameter = Animator.StringToHash("MoveSpeed");
        protected static readonly int isDeadAnimationParameter = Animator.StringToHash("IsDead");
        protected static readonly int isWinAnimationParameter = Animator.StringToHash("IsWin");
        protected static readonly int isLoseAnimationParameter = Animator.StringToHash("IsLose");
        protected Animator _animator;

        #endregion Class Methods

        #region Class Methods

        public virtual void RegisterAnimation()
        {
            _animator = gameObject.GetComponentInChildren<Animator>();
            ChangeMovementEvent += OnChangeMovementCallbackOnAnimation;
            WinEvent += OnWinCallbackOnAnimation;
            LoseEvent += OnLosenCallbackOnAnimation;
            // DieEvent += OnDienCallbackOnAnimation;
        }

        protected virtual void OnChangeMovementCallbackOnAnimation()
        {
            _animator.SetFloat(moveSpeedAnimationParameter, isMoving ? MoveSpeed : 0.0f);
        }

        protected virtual void OnWinCallbackOnAnimation()
        {
            _animator.SetTrigger(isWinAnimationParameter);
        }

        protected virtual void OnLosenCallbackOnAnimation()
        {
            _animator.SetTrigger(isLoseAnimationParameter);
        }

        protected virtual void OnDienCallbackOnAnimation()
        {
            _animator.SetTrigger(isDeadAnimationParameter);
        }

        #endregion Class Methods
    }
}