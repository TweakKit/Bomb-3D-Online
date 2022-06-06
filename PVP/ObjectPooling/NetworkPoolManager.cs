using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class NetworkPoolManager : MonoBehaviour
    {
        #region Members

        private static Dictionary<GameObject, NetworkPool> Pools = new Dictionary<GameObject, NetworkPool>();

        #endregion Members

        #region API Methods

        private void OnDestroy()
        {
            Pools.Clear();
        }

        #endregion API Methods

        #region Class Methods

        /// <summary>
        /// Called by each pool on its own, this adds it to the dictionary.
        /// </summary>
        public static void Add(NetworkPool pool)
        {
            if (pool.Prefab == null)
            {
                Debug.LogError("Prefab of pool: " + pool.gameObject.name + " is empty! Can't add pool to Pools Dictionary.");
                return;
            }

            if (Pools.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Pool with prefab " + pool.Prefab.name + " has already been added to Pools Dictionary.");
                return;
            }

            Pools.Add(pool.Prefab, pool);
        }

        /// <summary>
        /// Creates a new Pool at runtime. This is being called for prefabs which have not been linked
        /// to a Pool in the scene in the editor, but are called via Spawn() nonetheless.
        /// </summary>
        public static void CreatePool(GameObject prefab, int preLoad, bool limit, int maxCount)
        {
            if (Pools.ContainsKey(prefab))
            {
                Debug.LogError("Pool Manager already contains Pool for prefab: " + prefab.name);
                return;
            }

            GameObject poolGameObject = new GameObject("Pool " + prefab.name);
            NetworkPool pool = poolGameObject.AddComponent<NetworkPool>();
            pool.InitRuntime(prefab, preLoad, limit, maxCount);
            pool.Awake();
        }

        /// <summary>
        /// Activates a pre-instantiated instance for the prefab passed in, at the desired position.
        /// </summary>
        public static GameObject Spawn(GameObject prefab)
        {
            if (!Pools.ContainsKey(prefab))
            {
                Debug.Log("Prefab not found in existing pool: " + prefab.name + " New Pool has been created.");
                CreatePool(prefab, 0, false, 0);
            }

            return Pools[prefab].Spawn();
        }

        /// <summary>
        /// Disables a previously spawned instance for later use.
        /// Optionally takes a time value to delay the despawn routine.
        /// </summary>
        public static void Despawn(GameObject instance, float time = 0f)
        {
            if (time > 0)
                GetPool(instance).Despawn(instance, time);
            else
                GetPool(instance).Despawn(instance);
        }

        /// <summary>
        /// Convenience method for quick lookup of an pooled object.
        /// Returns the Pool component where the instance has been found in.
        /// </summary>
        public static NetworkPool GetPool(GameObject instance)
        {
            foreach (GameObject prefab in Pools.Keys)
            {
                if (Pools[prefab].active.Contains(instance))
                    return Pools[prefab];
            }

            Debug.LogError("PoolManager couldn't find Pool for instance: " + instance.name);
            return null;
        }

        /// <summary>
        /// Despawns all instances of a Pool, making them available for later use.
        /// </summary>
        public static void DeactivatePool(GameObject prefab)
        {
            if (!Pools.ContainsKey(prefab))
            {
                Debug.LogError("PoolManager couldn't find Pool for prefab to deactivate: " + prefab.name);
                return;
            }

            int count = Pools[prefab].active.Count;
            for (int i = count - 1; i >= 0; i--)
                Pools[prefab].Despawn(Pools[prefab].active[i]);
        }

        /// <summary>
        /// Destroys all despawned instances of all Pools to free up memory.
        /// The parameter 'limitToPreLoad' decides if only instances above the preload
        /// value should be destroyed, to keep a minimum amount of disabled instances.
        /// </summary>
        public static void DestroyAllInactive(bool limitToPreLoad)
        {
            foreach (GameObject prefab in Pools.Keys)
                Pools[prefab].DestroyUnused(limitToPreLoad);
        }

        /// <summary>
        /// Destroys the Pool for a specific prefab.
        /// Active or inactive instances are not available anymore after calling this.
        /// </summary>
        public static void DestroyPool(GameObject prefab)
        {
            if (!Pools.ContainsKey(prefab))
            {
                Debug.LogError("PoolManager couldn't find Pool for prefab to destroy: " + prefab.name);
                return;
            }

            Destroy(Pools[prefab].gameObject);
            Pools.Remove(prefab);
        }

        /// <summary>
        /// Destroys all pools stored in the manager dictionary.
        /// Active or inactive instances are not available anymore after calling this.
        /// </summary>
        public static void DestroyAllPools()
        {
            foreach (GameObject prefab in Pools.Keys)
                DestroyPool(Pools[prefab].gameObject);
        }

        #endregion Class Methods
    }
}