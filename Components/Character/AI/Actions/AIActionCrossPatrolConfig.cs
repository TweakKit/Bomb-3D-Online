using UnityEngine;

namespace ZB.Gameplay
{
    public class AIActionCrossPatrolConfig : AIActionConfig
    {
        #region Members

        [SerializeField]
        private AIActionCrossPatrol _AIActionCrossPatrol;

        #endregion Members

        #region Properties

        public override AIAction AIAction => _AIActionCrossPatrol;

        #endregion Properties
    }
}