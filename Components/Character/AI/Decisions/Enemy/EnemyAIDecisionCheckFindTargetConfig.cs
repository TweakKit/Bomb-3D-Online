using UnityEngine;

namespace ZB.Gameplay
{
    public class EnemyAIDecisionCheckFindTargetConfig : AIDecisionConfig
    {
        #region Members

        [SerializeField]
        private EnemyAIDecisionCheckFindTarget _enemyAIDecisionCheckFindTarget;

        #endregion Members

        #region Properties

        public override AIDecision AIDecision => _enemyAIDecisionCheckFindTarget;

        #endregion Properties
    }
}