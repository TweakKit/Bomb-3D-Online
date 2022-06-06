using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// A pool object holder holds a queue of pool objects, is reponsible for getting, returning, cleaning objects in the pool.
    /// </summary>
    public class PoolObjectHolder
    {
        #region Members

        private GameObject _objectPrefab;
        private Transform _parentTransform;
        private Enum _objectType;
        private int _poolSize;
        private Queue<GameObject> _poolObjects;

        #endregion Members

        #region Class Methods

        public PoolObjectHolder(GameObject objectPrefab, Transform parentTransform, Enum objectType, int poolSize)
        {
            _objectPrefab = objectPrefab;
            _parentTransform = parentTransform;
            _objectType = objectType;
            _poolSize = poolSize;
            _poolObjects = new Queue<GameObject>();
            CreatePool();
        }

        public GameObject GetObject(bool isActive)
        {
            if (_poolObjects.Count <= 0)
                CreatePool();

            GameObject returnedPoolObject = _poolObjects.Dequeue();
            returnedPoolObject.SetActive(isActive);
            return returnedPoolObject;
        }

        public void ReturnObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(_parentTransform);
            _poolObjects.Enqueue(gameObject);
        }

        public void CleanPool()
        {
            while (_poolObjects.Count > 0)
            {
                GameObject poolObject = _poolObjects.Dequeue();
                GameObject.Destroy(poolObject);
            }

            _poolObjects = null;
        }

        private void CreatePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject poolObject = GameObject.Instantiate(_objectPrefab);
                poolObject.transform.SetParent(_parentTransform);
                poolObject.SetActive(false);
                PoolObjectElement poolObjectElement = poolObject.GetOrAddComponent<PoolObjectElement>();
                poolObjectElement.ObjectType = _objectType;
                _poolObjects.Enqueue(poolObject);
            }
        }

        #endregion Class Methods
    }
}