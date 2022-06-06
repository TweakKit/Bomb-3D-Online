using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [System.Serializable]
    public class SceneNameField
    {
        #region Members

        [SerializeField]
        private Object _sceneAsset;
        [SerializeField]
        private string _sceneName;

        #endregion Members

        #region Properties

        public string SceneName { get { return _sceneName; } }

        #endregion Properties

        #region Class Methods

        public static implicit operator string(SceneNameField sceneNameField)
        {
            return sceneNameField.SceneName;
        }

        public bool IsSet()
        {
            return !string.IsNullOrEmpty(_sceneName);
        }

        #endregion Class Methods
    }
}