using System;

namespace ZB.Gameplay.PVP
{
    [Serializable]
    public class MagicHealPotionItemModel : ItemModel
    {
        #region Members

        public int healingHP;

        #endregion Members

        #region Class Methods

        public MagicHealPotionItemModel(MagicHealPotionItemModel other) : base(other)
        {
            healingHP = other.healingHP;
        }

        public override ItemModel Clone()
        {
            return new MagicHealPotionItemModel(this);
        }

        public override void Operate()
        {
            IPlayerAffectAction playerAffectAction = new MagicHealPotionAffectAction(healingHP, duration);
            Owner.ApplyAffectAction(playerAffectAction);
        }

        #endregion Class Methods
    }

    public class MagicHealPotionAffectAction : PlayerRuntimeAffectAction
    {
        #region Members

        private int _healingHP;

        #endregion Members

        #region Class Methods

        public MagicHealPotionAffectAction(int healingHP, float affectDuration) : base(affectDuration)
        {
            _healingHP = healingHP;
        }

        protected override void UpdatePerSecondTask()
        {
            base.UpdatePerSecondTask();
            _owner.AddCurrentHP(_healingHP);
        }

        #endregion Methods
    }
}