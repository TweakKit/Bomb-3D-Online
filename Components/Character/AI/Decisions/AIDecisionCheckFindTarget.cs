using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This decision will return true if the target character is detected in a specific distance.
    /// </summary>
    [Serializable]
    public abstract class AIDecisionCheckFindTarget : AIDecision
    {
        #region Members

        [SerializeField]
        protected bool _isTargetEnemy;
        protected float _checkDistanceSqr;
        protected List<CharacterModel> _targets;

        #endregion Members

        #region Class Methods

        public AIDecisionCheckFindTarget(AIDecisionCheckFindTarget other) : base(other) { }

        public override void Init(AIState ownerState, CharacterModel ownerModel)
        {
            base.Init(ownerState, ownerModel);
            SetCheckDistance();
            _targets = _isTargetEnemy ? EntitiesManager.Instance.Enemies : EntitiesManager.Instance.Heroes;
        }

        public override void Dispose()
        {
            base.Dispose();
            _targets = null;
        }

        public override bool Decide()
        {
            return EvaluateDistance();
        }

        protected virtual bool EvaluateDistance()
        {
            float minDistanceSqr = _checkDistanceSqr;
            CharacterModel nearTarget = null;

            foreach (var target in _targets)
            {
                float distanceSqr = Vector3.SqrMagnitude(target.Position - OwnerModel.Position);
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearTarget = target;
                }
            }

            if (nearTarget != null && MapManager.GetPathPositions(OwnerModel.Position, nearTarget.Position).HasPath())
            {
                OwnerModel.target = nearTarget;
                return true;
            }

            return false;
        }

        protected abstract void SetCheckDistance();

        #endregion Class Methods
    }
}