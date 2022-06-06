using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkPool : MonoBehaviour
    {
        #region Members

        [HideInInspector]
        public List<GameObject> active = new List<GameObject>();

        [HideInInspector]
        public List<GameObject> inactive = new List<GameObject>();

        [SerializeField]
        [Tooltip("Prefab to instantiate for pooling.")]
        private GameObject _prefab;

        [SerializeField]
        [Tooltip("Amount of instances to create at game start.")]
        private int _preLoad = 0;

        [SerializeField]
        [Tooltip("Whether the creation of new instances should be limited at runtime.")]
        private bool _limit = false;

        [SerializeField]
        [Tooltip("Maximum amount of instances created, if limit is enabled.")]
        private int _maxCount;

        #endregion Members

        #region Properties

        private int GetTotalCount
        {
            get
            {
                int count = active.Count + inactive.Count;
                return count;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return _prefab;
            }
        }

        #endregion Properties

        #region API Methods

        public void Awake()
        {
            if (_prefab == null)
                return;

            NetworkPoolManager.Add(this);
            NetworkIdentity networkPrefab = _prefab.GetComponent<NetworkIdentity>();
            if (networkPrefab != null)
                NetworkClient.RegisterSpawnHandler(networkPrefab.assetId, OnSpawn, Despawn);
        }

        private void Start()
        {
            PreLoad();
        }

        private void OnDestroy()
        {
            active.Clear();
            inactive.Clear();
        }

        #endregion API Methods

        #region Class Methods

        /// <summary>
        /// Loads specified amount of objects before playtime.
        /// </summary>
        public void PreLoad()
        {
            if (_prefab == null)
            {
                Debug.LogWarning("Prefab in pool empty! No Preload happening. Please check references.");
                return;
            }

            for (int i = GetTotalCount; i < _preLoad; i++)
            {
                GameObject obj = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(transform);
                Rename(obj.transform);
                obj.SetActive(false);
                inactive.Add(obj);
            }
        }

        /// <summary>
        /// Activates (or instantiates) a new instance for this pool.
        /// </summary>
        public GameObject Spawn()
        {
            GameObject obj;
            Transform trans;

            if (inactive.Count > 0)
            {
                obj = inactive[0];
                inactive.RemoveAt(0);
                trans = obj.transform;
            }
            else
            {
                if (_limit && active.Count >= _maxCount)
                    return null;

                obj = Instantiate(_prefab);
                trans = obj.transform;
                Rename(trans);
            }

            if (trans.parent != transform)
                trans.parent = transform;

            active.Add(obj);
            obj.SetActive(true);
            obj.BroadcastMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);

            return obj;
        }

        /// <summary>
        /// Deactivates an instance of this pool for later use.
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (!active.Contains(instance))
            {
                Debug.LogWarning("Can't despawn - Instance not found: " + instance.name + " in Pool " + this.name);
                return;
            }

            if (instance.transform.parent != transform)
                instance.transform.parent = transform;

            active.Remove(instance);
            inactive.Add(instance);
            instance.BroadcastMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);
            instance.SetActive(false);
        }

        /// <summary>
        /// Timed deactivation of an instance of this pool for later use.
        /// </summary>
        public void Despawn(GameObject instance, float time)
        {
            PoolTimeObject timeObject = new PoolTimeObject();
            timeObject.instance = instance;
            timeObject.deactivateDelay = time;
            StartCoroutine(DespawnInTime(timeObject));
        }

        /// <summary>
        /// Destroys all inactive instances of this pool (garbage collector heavy). 
        /// The parameter determines if only instances above the preLoad value should be destroyed.
        /// </summary>
        public void DestroyUnused(bool limitToPreLoad)
        {
            if (limitToPreLoad)
            {
                for (int i = inactive.Count - 1; i >= _preLoad; i--)
                    Destroy(inactive[i]);

                if (inactive.Count > _preLoad)
                    inactive.RemoveRange(_preLoad, inactive.Count - _preLoad);
            }
            else
            {
                for (int i = 0; i < inactive.Count; i++)
                    Destroy(inactive[i]);

                inactive.Clear();
            }
        }

        public void InitRuntime(GameObject prefab, int preLoad, bool limit, int maxCount)
        {
            _prefab = prefab;
            _preLoad = preLoad;
            _limit = limit;
            _maxCount = maxCount;
        }

        /// <summary>
        /// Called by Unity Networking's spawning system on Network.Spawn.
        /// </summary>
        private GameObject OnSpawn(Vector3 position, Guid assetId)
        {
            return Spawn();
        }

        /// <summary>
        /// Waits for 'time' seconds before deactivating the instance.
        /// </summary>
        private IEnumerator DespawnInTime(PoolTimeObject timeObject)
        {
            GameObject instance = timeObject.instance;

            float timer = Time.time + timeObject.deactivateDelay;
            while (instance.activeInHierarchy && Time.time < timer)
                yield return null;

            if (!instance.activeInHierarchy)
                yield break;

            Despawn(instance);
        }

        /// <summary>
        /// Create a unique name for each instance at instantiation to differ them from each other in the editor.
        /// </summary>
        private void Rename(Transform instance)
        {
            instance.name += (GetTotalCount + 1).ToString("#000");
        }

        #endregion Class Methods
    }

    public class PoolTimeObject
    {
        #region Members

        public GameObject instance;
        public float deactivateDelay;

        #endregion Members
    }
}