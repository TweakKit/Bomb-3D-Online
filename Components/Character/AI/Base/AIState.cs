using System;
using System.Collections.Generic;
using System.Linq;

namespace ZB.Gameplay
{
    /// <summary>
    /// A State is a combination of one or more actions, and one or more transitions. 
    /// An example of a state could be "Patrolling until another character gets in range".
    /// </summary>
    [Serializable]
    public class AIState
    {
        #region Members

        private string _stateName;
        private List<AIAction> _actions;
        private List<AITransition> _transitions;

        #endregion Members

        #region Properties

        public string StateName => _stateName;

        #endregion Properties

        #region Class Methods

        public AIState(string stateName, List<AIAction> actions, List<AITransition> transitions)
        {
            _stateName = stateName;
            _actions = actions;
            _transitions = transitions;
        }

        public AIState Clone()
        {
            List<AIAction> clonedActions = _actions.Select(x => x.Clone()).ToList();
            List<AITransition> clonedTransition = _transitions.Select(x => x.Clone()).ToList();
            return new AIState(_stateName, clonedActions, clonedTransition);
        }

        /// <summary>
        /// Set the brain's owner model for the state.
        /// </summary>
        public void Init(CharacterModel ownerModel)
        {
            _actions.ForEach(x => x.Init(this, ownerModel));
            _transitions.ForEach(x => x.Init(this, ownerModel));
        }

        public void Reset()
        {
            _actions.ForEach(x => x.Dispose());
            _transitions.ForEach(x => x.Dispose());
        }

        /// <summary>
        /// On enter state we pass that info to our actions and decisions.
        /// </summary>
        public void EnterState()
        {
            foreach (AIAction action in _actions)
                action.OnEnterState();

            foreach (AITransition transition in _transitions)
                if (transition.Decision != null)
                    transition.Decision.OnEnterState();
        }

        /// <summary>
        /// On exit state we pass that info to our actions and decisions.
        /// </summary>
        public void ExitState()
        {
            foreach (AIAction action in _actions)
                action.OnExitState();

            foreach (AITransition transition in _transitions)
                if (transition.Decision != null)
                    transition.Decision.OnExitState();
        }

        /// <summary>
        /// Perform this state's actions.
        /// </summary>
        public void PerformActions()
        {
            for (int i = 0; i < _actions.Count; i++)
                _actions[i].PerformAction();
        }

        /// <summary>
        /// Evaluate this state's transitions.
        /// </summary>
        public string EvaluateTransitions()
        {
            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].Decision.Decide())
                    return _transitions[i].TrueState;
                else
                    return _transitions[i].FalseState;
            }

            return null;
        }

        #endregion Class Methods
    }
}