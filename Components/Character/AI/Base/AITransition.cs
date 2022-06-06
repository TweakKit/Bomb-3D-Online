using System;

namespace ZB.Gameplay
{
    /// <summary>
    /// Transitions are a combination of one or more decisions and destination states whether or not these transitions are true or false.
    /// An example of a transition could be "If another character gets in range, transition to the Placing Bomb state".
    /// </summary>
    [Serializable]
    public class AITransition
    {
        #region Members

        private AIDecision _decision;
        private string _trueState;
        private string _falseState;

        #endregion Members

        #region Properties

        public AIDecision Decision => _decision;
        public string TrueState => _trueState;
        public string FalseState => _falseState;

        #endregion Properties

        #region Class Methods

        public AITransition(AIDecision decision, string trueState, string falseState)
        {
            _decision = decision;
            _trueState = trueState;
            _falseState = falseState;
        }

        public AITransition Clone()
        {
            AIDecision clonedDecision = _decision.Clone();
            return new AITransition(clonedDecision, _trueState, _falseState);
        }

        public void Init(AIState ownerState, CharacterModel ownerModel)
        {
            _decision.Init(ownerState, ownerModel);
        }

        public void Dispose()
        {
            _decision.Dispose();
        }

        #endregion Class Methods
    }
}