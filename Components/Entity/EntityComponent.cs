using UnityEngine;

namespace ZB.Gameplay
{
    public abstract class EntityComponent<T> : MonoBehaviour where T : EntityModel
    {
        #region Class Methods

        public virtual void InitModel(T model) { }

        #endregion Class Methods
    }
}