using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    public class AIGroupStatesConfig : ScriptableObject
    {
        #region Members

        [SerializeField]
        private List<AIStateConfig> _stateConfigs;

        #endregion Members

        #region Class Methods

        public List<AIState> GetStates()
        {
            return _stateConfigs.Select(x => x.AIState).ToList();
        }

        #endregion Class Methods
    }
}