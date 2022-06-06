using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class RocketLaunchItemModel : ItemModel
    {
        #region Members

        public GameObject rocketLaunchItemObjectPrefab;

        #endregion Members

        #region Class Methods

        public RocketLaunchItemModel(RocketLaunchItemModel other) : base(other)
        {
            rocketLaunchItemObjectPrefab = other.rocketLaunchItemObjectPrefab;
        }

        public override ItemModel Clone()
        {
            return new RocketLaunchItemModel(this);
        }

        public override void Operate()
        {
            Owner.UseRocketLaunchItem(rocketLaunchItemObjectPrefab);
        }

        #endregion Class Methods
    }
}