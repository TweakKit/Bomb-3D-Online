using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    /// <summary>
    /// This helps filter which pool object creators can be initialized or destroyed since some pools are in need of initialization,
    /// some are not depending on the game context.
    /// </summary>
    public class PoolsInitChecker : MonoBehaviour
    {
        #region Properties

        public Enum PoolType
        {
            get
            {
                PoolObjectCreator[] poolObjectCreators = gameObject.GetComponentsInChildren<PoolObjectCreator>();
                if (poolObjectCreators.Length > 0)
                    return poolObjectCreators[0].ObjectType;
                else
                    return default(Enum);
            }
        }

        #endregion Properties

        #region Class Methods

        public void Init(List<Enum> objectType)
        {
            PoolObjectCreator[] poolObjectCreators = gameObject.GetComponentsInChildren<PoolObjectCreator>();
            for (int i = poolObjectCreators.Length - 1; i >= 0; i--)
            {
                if (objectType.Any(x => x.Equals(poolObjectCreators[i].ObjectType)))
                    poolObjectCreators[i].Init();
                else
                    Destroy(poolObjectCreators[i].gameObject);
            }
        }

        #endregion Class Methods
    }
}