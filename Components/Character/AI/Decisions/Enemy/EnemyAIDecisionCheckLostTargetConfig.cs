using UnityEngine;

namespace ZB.Gameplay
{
    public class EnemyAIDecisionCheckLostTargetConfig : AIDecisionConfig
    {
        #region Members

        [SerializeField]
        private EnemyAIDecisionCheckLostTarget _enemyAIDecisionCheckLostTarget;

        #endregion Members

        #region Properties

        public override AIDecision AIDecision => _enemyAIDecisionCheckLostTarget;

        #endregion Properties
    }
}