using System;
using UnityEngine;

namespace ZB.Gameplay
{
    [Serializable]
    public class AIDecisionCheckHasChestConfig : AIDecisionConfig
    {
        #region Members

        [SerializeField]
        private AIDecisionCheckHasChest _AIDecisionCheckHasChest;

        #endregion Members

        #region Properties

        public override AIDecision AIDecision => _AIDecisionCheckHasChest;

        #endregion Properties
    }
}