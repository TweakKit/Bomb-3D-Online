using UnityEngine;

namespace ZB.Gameplay
{
    public class AIActionPatrolConfig : AIActionConfig
    {
        #region Members

        [SerializeField]
        private AIActionPatrol _AIActionPatrol;

        #endregion Members

        #region Properties

        public override AIAction AIAction => _AIActionPatrol;

        #endregion Properties
    }
}