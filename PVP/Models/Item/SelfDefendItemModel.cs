using System;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class SelfDefendItemModel : ItemModel
    {
        #region Class Methods

        public SelfDefendItemModel(SelfDefendItemModel other) : base(other) { }

        public override ItemModel Clone()
        {
            return new SelfDefendItemModel(this);
        }

        public override void Operate()
        {
            IPlayerAffectAction playerAffectAction = new SelfDefendAffectAction(duration);
            Owner.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class SelfDefendAffectAction : PlayerSimpleAffectAction
    {
        #region Class Methods

        public SelfDefendAffectAction(float affectDuration) : base(affectDuration) { }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.TriggerSelfDefend();
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            _owner.ResetSelfDefend();
        }

        #endregion Methods
    }
}