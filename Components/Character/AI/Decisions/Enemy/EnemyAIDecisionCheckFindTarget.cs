using System;

namespace ZB.Gameplay
{
    [Serializable]
    public class EnemyAIDecisionCheckFindTarget : AIDecisionCheckFindTarget
    {
        #region Class Methods

        public EnemyAIDecisionCheckFindTarget(EnemyAIDecisionCheckFindTarget other) : base(other) { }

        public override AIDecision Clone() => new EnemyAIDecisionCheckFindTarget(this);

        protected override void SetCheckDistance()
        {
            EnemyModel enemyModel = OwnerModel as EnemyModel;
            _checkDistanceSqr = enemyModel.ChaseTargetAOE * MapSetting.MapSquareSize * enemyModel.ChaseTargetAOE * MapSetting.MapSquareSize;
        }

        #endregion Class Methods
    }
}