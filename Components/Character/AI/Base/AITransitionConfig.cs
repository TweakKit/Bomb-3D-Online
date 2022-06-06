using UnityEngine;

namespace ZB.Gameplay
{
    public class AITransitionConfig : ScriptableObject
    {
        #region Members

        [Tooltip("The dicision of the transition.")]
        [SerializeField]
        private AIDecisionConfig _decisionConfig;

        [Tooltip("The state to transition to if this Decision returns true.")]
        [SerializeField]
        private string _trueState;

        [Tooltip("The state to transition to if this Decision returns false.")]
        [SerializeField]
        private string _falseState;

        #endregion Members

        #region Properties

        public AITransition AITransition => new AITransition(_decisionConfig.AIDecision, _trueState, _falseState);

        #endregion Properties
    }
}