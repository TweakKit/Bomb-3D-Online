using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    [Serializable]
    public class AIActionConfigsList : ReorderableArray<AIActionConfig>
    {
        #region Class Methods

        public List<AIAction> GetAIActions()
        {
            return _array.Select(x => x.AIAction).ToList();
        }

        #endregion Class Methods
    }

    [Serializable]
    public class AITransitionConfigsList : ReorderableArray<AITransitionConfig>
    {
        #region Class Methods

        public List<AITransition> GetAITransitions()
        {
            return _array.Select(x => x.AITransition).ToList();
        }

        #endregion Class Methods
    }


    public class AIStateConfig : ScriptableObject
    {
        #region Members

        [Header("The name of the state.")]
        [Tooltip("The name of the state (will be used as a reference in Transitions).")]
        [SerializeField]
        private string _stateName;

        [Header("State actions.")]
        [SerializeField]
        private AIActionConfigsList _actionConfigs;

        [Header("State transitions.")]
        [SerializeField]
        private AITransitionConfigsList _transitionConfigs;

        #endregion Members

        #region Properties

        public AIState AIState => new AIState(_stateName, _actionConfigs.GetAIActions(), _transitionConfigs.GetAITransitions());

        #endregion Properties
    }
}