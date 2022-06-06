using UnityEngine;

namespace ZB.Gameplay
{
    public class AIActionMoveFollowTargetConfig : AIActionConfig
    {
        #region Members

        [SerializeField]
        private AIActionMoveFollowTarget _AIActionMoveFollowTarget;

        #endregion Members

        #region Properties

        public override AIAction AIAction => _AIActionMoveFollowTarget;

        #endregion Properties
    }
}