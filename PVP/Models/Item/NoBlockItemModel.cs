using System;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class NoBlockItemModel : ItemModel
    {
        #region Class Methods

        public NoBlockItemModel(NoBlockItemModel other) : base(other) { }

        public override ItemModel Clone()
        {
            return new NoBlockItemModel(this);
        }

        public override void Operate()
        {
            IPlayerAffectAction playerAffectAction = new NoBlockAffectAction(duration);
            Owner.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class NoBlockAffectAction : PlayerSimpleAffectAction
    {
        #region Class Methods

        public NoBlockAffectAction(float affectDuration) : base(affectDuration) { }

        protected override void StartTask()
        {
            base.StartTask();
            // TODO.
        }

        protected override void FinishTask()
        {
            base.FinishTask();
            // TODO.
        }

        #endregion Methods
    }
}