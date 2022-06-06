using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class TNTExplosiveItemModel : ItemModel
    {
        #region Members

        [Min(1)]
        public int explodeRange;
        public float damageValue;
        public float explosionTime;
        public GameObject bombTNTExplosivePrefab;

        #endregion Members

        #region Class Methods

        public TNTExplosiveItemModel(TNTExplosiveItemModel other) : base(other)
        {
            explodeRange = other.explodeRange;
            damageValue = other.damageValue;
            bombTNTExplosivePrefab = other.bombTNTExplosivePrefab;
        }

        public override ItemModel Clone()
        {
            return new TNTExplosiveItemModel(this);
        }

        public override void Operate()
        {
            NetworkedBombData bombData = new NetworkedBombData(BombType.BombTNTExplosive, explosionTime, damageValue, explodeRange);
            Owner.UseTNTExplosiveItem(bombData, bombTNTExplosivePrefab);
        }

        #endregion Class Methods
    }
}