using System;

namespace ZB.Gameplay
{
    [Serializable]
    public class EnemyAIDecisionCheckLostTarget : AIDecisionCheckLostTarget
    {
        #region Class Methods

        public EnemyAIDecisionCheckLostTarget(EnemyAIDecisionCheckLostTarget other) : base(other) { }

        public override AIDecision Clone() => new EnemyAIDecisionCheckLostTarget(this);

        protected override void SetCheckDistance()
        {
            EnemyModel enemyModel = OwnerModel as EnemyModel;
            _checkDistanceSqr = ((enemyModel.LoseTargetAOE + 1) * MapSetting.MapSquareSize) * ((enemyModel.LoseTargetAOE + 1) * MapSetting.MapSquareSize);
        }

        #endregion Class Methods
    }
}