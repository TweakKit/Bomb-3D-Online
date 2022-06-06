using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class SlowTrapItemModel : ItemModel
    {
        #region Members

        public float dropTime;
        public GameObject slowTrapItemObjectPrefab;

        #endregion Members

        #region Class Methods

        public SlowTrapItemModel(SlowTrapItemModel other) : base(other)
        {
            dropTime = other.dropTime;
            slowTrapItemObjectPrefab = other.slowTrapItemObjectPrefab;
        }

        public override ItemModel Clone()
        {
            return new SlowTrapItemModel(this);
        }

        public override void Operate()
        {
            Owner.UseSlowTrapItem(slowTrapItemObjectPrefab);
        }

        #endregion Class Methods
    }
}