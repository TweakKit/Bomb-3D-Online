namespace ZB.Gameplay
{
    public abstract class MapItemComponent : EntityComponent<MapItemModel>
    {
        #region Members

        protected MapItemModel _model;

        #endregion Members

        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);
            _model = model;
        }

        #endregion Class Methods
    }
}