using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    #region Members

    protected static T instance;
    protected static object locker = new object();

    #endregion Members

    #region Properties

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (locker)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject instanceGameObject = new GameObject();
                        instance = instanceGameObject.AddComponent<T>();
                        instanceGameObject.name = typeof(T).Name;
                    }
                }
            }

            return instance;
        }
    }

    #endregion Properties

    #region API Methods

    protected virtual void Awake()
    {
        lock (locker)
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (this != instance)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion API Methods
}