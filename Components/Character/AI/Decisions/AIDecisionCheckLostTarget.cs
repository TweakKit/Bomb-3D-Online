using System;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This decision will return true if the target was chased is now lost in a specific distance.
    /// </summary>
    [Serializable]
    public abstract class AIDecisionCheckLostTarget : AIDecision
    {
        #region Members

        protected float _checkDistanceSqr;

        #endregion Members

        #region Class Methods

        public AIDecisionCheckLostTarget(AIDecisionCheckLostTarget other) : base(other) { }

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            SetCheckDistance();
        }

        public override void OnExitState()
        {
            base.OnExitState();
            OwnerModel.target = null;
        }

        public override bool Decide()
        {
            return EvaluateDistance();
        }

        protected virtual bool EvaluateDistance()
        {
            if (OwnerModel.target.IsDead)
                return true;
            else
                return Vector3.SqrMagnitude(OwnerModel.target.Position - OwnerModel.Position) > _checkDistanceSqr;
        }

        protected abstract void SetCheckDistance();

        #endregion Class Methods
    }
}