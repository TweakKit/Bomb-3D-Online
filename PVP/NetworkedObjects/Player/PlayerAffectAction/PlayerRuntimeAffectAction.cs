using UnityEngine;

namespace ZB.Gameplay.PVP
{
    /// <summary>
    /// Runtime affect action also runs tasks in every frame or/and per second.
    /// </summary>
    public class PlayerRuntimeAffectAction : PlayerSimpleAffectAction
    {
        #region Members

        protected int _affectPerSecondTimes;

        #endregion Members

        #region Class Methods

        public PlayerRuntimeAffectAction(float affectDuration) : base(affectDuration)
        {
            _affectPerSecondTimes = Mathf.CeilToInt(affectDuration);
        }

        public override void Update()
        {
            UpdatePerFrameTask();
            CurrentAffectDuration -= Time.deltaTime;

            if (_affectPerSecondTimes > 0 && _affectPerSecondTimes > Mathf.CeilToInt(CurrentAffectDuration))
            {
                _affectPerSecondTimes--;
                UpdatePerSecondTask();
            }

            if (CurrentAffectDuration <= 0)
            {
                FinishTask();
                HasFinished = true;
            }
        }

        protected virtual void UpdatePerFrameTask() { }
        protected virtual void UpdatePerSecondTask() { }

        #endregion Class Methods
    }
}