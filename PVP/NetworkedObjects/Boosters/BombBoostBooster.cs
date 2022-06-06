using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class BombBoostBooster : TimeLimitBooster
    {
        #region Membes

        [SerializeField]
        private int _bonusBombNumber = 3;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            IPlayerAffectAction playerAffectAction = new BombBoostAffectAction(_bonusBombNumber, _usageLimitTime);
            networkedPlayer.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class BombBoostAffectAction : PlayerSimpleAffectAction
    {
        #region Members

        private int _bonusBombNumber;

        #endregion Members

        #region Class Methods

        public BombBoostAffectAction(int bonusBombNumber, float affectDuration) : base(affectDuration)
        {
            _bonusBombNumber = bonusBombNumber;
        }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.AddBombNumber(_bonusBombNumber);
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            _owner.SubtractBombNumber(_bonusBombNumber);
        }

        #endregion Methods
    }
}