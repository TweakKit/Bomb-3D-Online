using UnityEngine;

namespace ZB.Gameplay
{
    public abstract class AIActionConfig : ScriptableObject
    {
        #region Properties

        public abstract AIAction AIAction { get; }

        #endregion Properties

        #region API methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            AIAction.Validate();
        }
#endif

        #endregion API Methods
    }
}