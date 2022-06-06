using System;
using UnityEngine;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class FlyBootsItemModel : ItemModel
    {
        #region Members

        [Range(0, 100)]
        public int speedPercentBoost;

        #endregion Members

        #region Class Methods

        public FlyBootsItemModel(FlyBootsItemModel other) : base(other)
        {
            speedPercentBoost = other.speedPercentBoost;
        }

        public override ItemModel Clone()
        {
            return new FlyBootsItemModel(this);
        }

        public override void Operate()
        {
            IPlayerAffectAction playerAffectAction = new FlyBootsAffectAction(speedPercentBoost, duration);
            Owner.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class FlyBootsAffectAction : PlayerSimpleAffectAction
    {
        #region Members

        private float _speedPercentBoost;

        #endregion Members

        #region Class Methods

        public FlyBootsAffectAction(float speedPercentBoost, float affectDuration) : base(affectDuration)
        {
            _speedPercentBoost = speedPercentBoost;
        }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.AddSpeedPercent(_speedPercentBoost);
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            _owner.SubtractSpeedPercent(_speedPercentBoost);
        }

        #endregion Methods
    }
}