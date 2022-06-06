using ZB.Model;

namespace ZB.Gameplay
{
    public class HeroModel : CharacterModel
    {
        #region Properties

        public override bool IsHero { get { return true; } }
        public override bool IsEnemy { get { return false; } }
        public override bool IsControlled { get { return false; } }

        #endregion Properties

        #region Class Methods

        public HeroModel(InventoryItem characterData) : base(characterData)
        {
            AIstates = IsControlled ? null : DataManager.Instance.GetAutoPlayHeroAIStates();
        }

        #endregion Class Methods
    }
}