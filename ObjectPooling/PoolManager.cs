using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    public class PoolManager : Singleton<PoolManager>
    {
        #region Properties

        protected Dictionary<Enum, PoolObjectHolder> PoolObjectsDictionary { get; set; }

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        #endregion API Methods

        #region Class Methods

        public virtual void Initialize()
        {
            Callback OnLoadedLevel = () =>
            {
                PoolObjectsDictionary = new Dictionary<Enum, PoolObjectHolder>();
                PoolsInitChecker[] poolsInitCheckers = gameObject.GetComponentsInChildren<PoolsInitChecker>();

                List<Enum> bombTypes = EntitiesManager.Instance.Characters.Select(x => (Enum)x.bombData.bombType).Distinct().ToList();
                if (LevelLoader.HasBigBoomSecret)
                    bombTypes.Add(BombType.BombBigSecret);

                List<Enum> bombExplosionEffectTypes = bombTypes.Select(x => (Enum)((BombType)x).GetBombExplosionEffectType()).Distinct().ToList();

                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<PoolsInitChecker>() != null)
                    {
                        PoolsInitChecker poolsInitChecker = transform.GetChild(i).GetComponent<PoolsInitChecker>();
                        if (poolsInitChecker.PoolType is BombType)
                            poolsInitChecker.Init(bombTypes);
                        else if (poolsInitChecker.PoolType is BombExplosionEffectType)
                            poolsInitChecker.Init(bombExplosionEffectTypes);
                    }
                    else
                    {
                        PoolObjectCreator[] poolObjectCreators = transform.GetChild(i).GetComponentsInChildren<PoolObjectCreator>();
                        foreach (var poolObjectCreator in poolObjectCreators)
                            poolObjectCreator.Init();
                    }
                }
            };

            EventManager.AddListener(GameEventType.LoadedLevel, OnLoadedLevel);
        }

        public static void CreatePool(GameObject objectPrefab, Transform parentTransform, Enum objectType, int poolSize)
        {
            if (Instance.PoolObjectsDictionary.ContainsKey(objectType))
            {
#if UNITY_EDITOR
                Debug.LogError("A pool of the same object type has been created!");
#endif
                return;
            }

            PoolObjectHolder poolObjectHolder = new PoolObjectHolder(objectPrefab, parentTransform, objectType, poolSize);
            Instance.PoolObjectsDictionary.Add(objectType, poolObjectHolder);
        }

        public static GameObject GetObject(Enum objectType, bool isActive = true)
        {
#if UNITY_EDITOR
            if (!Instance.PoolObjectsDictionary.ContainsKey(objectType))
            {
                Debug.LogError("There is no pool of this object type! " + objectType);
                return null;
            }
#endif
            PoolObjectHolder poolObjectHolder = Instance.PoolObjectsDictionary[objectType];
            return poolObjectHolder.GetObject(isActive);
        }

        public static void ReturnObject(GameObject gameObject)
        {
            Enum objectType = gameObject.GetComponent<PoolObjectElement>().ObjectType;
#if UNITY_EDITOR
            if (!Instance.PoolObjectsDictionary.ContainsKey(objectType))
            {
                Debug.LogError("There is no pool of this object type! " + objectType);
                return;
            }
#endif
            PoolObjectHolder poolObjectHolder = Instance.PoolObjectsDictionary[objectType];
            poolObjectHolder.ReturnObject(gameObject);
        }

        public static void CleanPool()
        {
            foreach (KeyValuePair<Enum, PoolObjectHolder> keyValuePair in Instance.PoolObjectsDictionary)
            {
                PoolObjectHolder poolObjectHolder = keyValuePair.Value;
                poolObjectHolder.CleanPool();
            }

            Instance.PoolObjectsDictionary = null;
        }

        #endregion Class Methods
    }
}