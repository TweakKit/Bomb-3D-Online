using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This component is responsible for going from one state to the other based on the defined transitions. 
    /// It's basically just a collection of states, and it's where all the actions, decisions, states and transitions are linked together.
    /// Attach this component to a character will allow it to work with AI behaviors.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterAIBrainComponent : CharacterComponent
    {
        #region Members

        protected List<AIState> _AIstates;

        #endregion Members

        #region Properties

        protected AIState CurrentState { get; set; }

        #endregion Properties

        #region API Methods

        protected virtual void Update()
        {
            UpdateComponent();
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);

            if (model.AIstates != null)
            {
                InitBrain(model.AIstates);
                _model.DieEvent += OnDie;
            }
            else Destroy(this);
        }

        /// <summary>
        /// Init the brain, forcing it to enter its first state.
        /// </summary>
        protected virtual void InitBrain(List<AIState> AIstates)
        {
            _AIstates = AIstates;
            _AIstates.ForEach(x => x.Init(_model));
            if (_AIstates.Count > 0)
            {
                CurrentState = _AIstates[0];
                CurrentState.EnterState();
            }
        }

        protected virtual void OnDie()
        {
            _AIstates.ForEach(x => x.Reset());
            _AIstates = null;
        }

        protected virtual void UpdateComponent()
        {
            if (_model.isActivated && !_model.IsDead)
            {
                UpdateAction();
                CheckTransition();
            }
        }

        protected virtual void UpdateAction()
        {
            CurrentState.PerformActions();
        }

        protected virtual void CheckTransition()
        {
            string transitionedStateName = CurrentState.EvaluateTransitions();
            if (!string.IsNullOrEmpty(transitionedStateName))
                TransitionToState(transitionedStateName);
        }

        /// <summary>
        /// Transition to the specified state, trigger exit and enter states events.
        /// </summary>
        protected virtual void TransitionToState(string newStateName)
        {
            if (newStateName != CurrentState.StateName)
            {
                CurrentState.ExitState();
                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                    CurrentState.EnterState();
            }
        }

        /// <summary>
        /// Returns a state based on the specified state name.
        /// </summary>
        protected virtual AIState FindState(string stateName)
        {
            foreach (AIState state in _AIstates)
                if (state.StateName == stateName)
                    return state;

#if UNITY_EDITOR
            if (stateName != "")
                Debug.LogError("You're trying to transition to state '" + stateName + "' in " + this.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
#endif
            return null;
        }

        #endregion Class Methods
    }
}