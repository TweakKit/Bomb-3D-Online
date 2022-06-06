using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public abstract class ItemModelConfig : ScriptableObject
    {
        #region Properties

        protected abstract ItemModel ItemModel { get; }

        #endregion Properties

        #region API Methods

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            ItemModel.Validate();
        }
#endif

        #endregion API Methods
    }
}