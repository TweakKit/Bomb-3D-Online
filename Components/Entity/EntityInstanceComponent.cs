using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// Every entity has to have an instance component in order to work.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class EntityInstanceComponent<T> : MonoBehaviour where T : EntityModel
    {
        #region Class Methods

        public virtual void Build(T model, Vector3 worldPosition)
        {
            transform.position = worldPosition;
            Init(model);
        }

        protected virtual void Init(T model)
        {
            model.ownerTransform = transform;
            model.visualTransform = transform.FindChildByName(Constants.EntityVisualName);
        }

        #endregion Class Methods
    }
}