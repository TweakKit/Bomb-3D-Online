using System;

namespace ZB.Gameplay
{
    /// <summary>
    /// This decision will return true if there is no chest left.
    /// </summary>
    [Serializable]
    public class AIDecisionCheckNoChest : AIDecision
    {
        #region Class Methods

        public AIDecisionCheckNoChest(AIDecisionCheckNoChest other) : base(other) { }

        public override AIDecision Clone() => new AIDecisionCheckNoChest(this);

        public override bool Decide()
        {
            return !EntitiesManager.Instance.HasChest;
        }

        #endregion Class Methods
    }
}