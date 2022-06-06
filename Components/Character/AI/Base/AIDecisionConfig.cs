using UnityEngine;

namespace ZB.Gameplay
{
    public abstract class AIDecisionConfig : ScriptableObject
    {
        #region Properties

        public abstract AIDecision AIDecision { get; }

        #endregion Properties

        #region API methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            AIDecision.Validate();
        }
#endif

        #endregion API Methods
    }
}