using System;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay
{
    public abstract class PoolObjectCreator : MonoBehaviour
    {
        #region Members

        [SerializeField]
        protected GameObject _objectPrefab;
        [SerializeField]
        protected int _size;

        #endregion Members

        #region Properties

        public abstract Enum ObjectType { get; }

        #endregion Properties

        #region Class Methods

        public virtual void Init()
        {
            NetworkIdentity networkEntity = _objectPrefab.GetComponent<NetworkIdentity>();
            if (networkEntity != null && !NetworkClient.prefabs.ContainsValue(_objectPrefab))
                NetworkClient.RegisterPrefab(_objectPrefab);

            PoolManager.CreatePool(_objectPrefab, transform, ObjectType, _size);
        }

        #endregion Class Methods
    }
}