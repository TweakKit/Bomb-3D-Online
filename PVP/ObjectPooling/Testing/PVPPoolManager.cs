using System;
using System.Collections.Generic;

namespace ZB.Gameplay.PVP
{
    public class PVPPoolManager : Gameplay.PoolManager
    {
        #region Class Methods

        public override void Initialize()
        {
            PoolObjectsDictionary = new Dictionary<Enum, PoolObjectHolder>();

            // TODO: Filter which pool object creators can be initialized or destroyed since some pools are in need of initialization,
            // some are not depending on the game context (as in the PVP mode). For now just initialize them all.
            for (int i = 0; i < transform.childCount; i++)
            {
                PoolObjectCreator[] poolObjectCreators = transform.GetChild(i).GetComponentsInChildren<PoolObjectCreator>();
                foreach (var poolObjectCreator in poolObjectCreators)
                    poolObjectCreator.Init();
            }   
        }

        #endregion Class Methods
    }
}