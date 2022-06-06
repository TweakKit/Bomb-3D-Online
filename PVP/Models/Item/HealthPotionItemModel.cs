using System;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class HealthPotionItemModel : ItemModel
    {
        #region Members

        public int healingHP;

        #endregion Members

        #region Class Methods

        public HealthPotionItemModel(HealthPotionItemModel other) : base(other)
        {
            healingHP = other.healingHP;
        }

        public override ItemModel Clone()
        {
            return new HealthPotionItemModel(this);
        }

        public override void Operate()
        {
            IPlayerAffectAction playerAffectAction = new HealthPotionAffectAction(healingHP, duration);
            Owner.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class HealthPotionAffectAction : PlayerRuntimeAffectAction
    {
        #region Members

        private int _healingHP;

        #endregion Members

        #region Class Methods

        public HealthPotionAffectAction(int healingHP, float affectDuration) : base(affectDuration)
        {
            _healingHP = healingHP;
        }

        protected override void StartTask()
        {
            base.StartTask();
            _owner.AddCurrentHP(_healingHP);
        }

        #endregion Methods
    }
}