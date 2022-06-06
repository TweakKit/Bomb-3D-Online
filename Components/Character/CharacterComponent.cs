namespace ZB.Gameplay
{
    public abstract class CharacterComponent : EntityComponent<CharacterModel>
    {
        #region Members

        protected CharacterModel _model;

        #endregion Members

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);
            _model = model;
        }

        #endregion Class Methods
    }
}