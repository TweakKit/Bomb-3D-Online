using System;
using UnityEngine;

namespace ZB.Gameplay
{
    [Serializable]
    public class AIDecisionCheckNoChestConfig : AIDecisionConfig
    {
        #region Members

        [SerializeField]
        private AIDecisionCheckNoChest _AIDecisionCheckNoChest;

        #endregion Members

        #region Properties

        public override AIDecision AIDecision => _AIDecisionCheckNoChest;

        #endregion Properties
    }
}