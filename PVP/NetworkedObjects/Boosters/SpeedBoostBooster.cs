using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class SpeedBoostBooster : TimeLimitBooster
    {
        #region Membes

        [SerializeField]
        private float _bonusSpeedValue = 2.0f;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            IPlayerAffectAction playerAffectAction = new SpeedBoostAffectAction(_bonusSpeedValue, _usageLimitTime);
            networkedPlayer.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class SpeedBoostAffectAction : PlayerSimpleAffectAction
    {
        #region Members

        private float _bonusSpeedValue;

        #endregion Members

        #region Class Methods

        public SpeedBoostAffectAction(float bonusSpeedValue, float affectDuration) : base(affectDuration)
        {
            _bonusSpeedValue = bonusSpeedValue;
        }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.AddSpeedValue(_bonusSpeedValue);
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            _owner.SubtractSpeedValue(_bonusSpeedValue);
        }

        #endregion Methods
    }
}