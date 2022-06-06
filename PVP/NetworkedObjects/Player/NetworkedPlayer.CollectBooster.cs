namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Class Methods

        public virtual void RegisterCollectBooster()
        {
            CollectBoosterEvent += OnCollectBooster_CallbackOnCollectBooster;
        }

        protected virtual void OnCollectBooster_CallbackOnCollectBooster(Booster booster)
        {
            booster.GetCollected(this);
        }

        #endregion Class Methods
    }
}