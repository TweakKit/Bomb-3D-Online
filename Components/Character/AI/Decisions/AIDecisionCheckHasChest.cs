using System;

namespace ZB.Gameplay
{
    /// <summary>
    /// This decision will return true if there is still chest remaining.
    /// </summary>
    [Serializable]
    public class AIDecisionCheckHasChest : AIDecision
    {
        #region Class Methods

        public AIDecisionCheckHasChest(AIDecisionCheckHasChest other) : base(other) { }

        public override AIDecision Clone() => new AIDecisionCheckHasChest(this);

        public override bool Decide()
        {
            return EntitiesManager.Instance.HasChest;
        }

        #endregion Class Methods
    }
}