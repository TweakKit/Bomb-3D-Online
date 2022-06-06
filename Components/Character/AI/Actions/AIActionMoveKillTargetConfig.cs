using UnityEngine;

namespace ZB.Gameplay
{
    public class AIActionMoveKillTargetConfig : AIActionConfig
    {
        #region Members

        [SerializeField]
        private AIActionMoveKillTarget _AIActionMoveKillTarget;

        #endregion Members

        #region Properties

        public override AIAction AIAction => _AIActionMoveKillTarget;

        #endregion Properties
    }
}