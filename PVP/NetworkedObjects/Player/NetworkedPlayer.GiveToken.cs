namespace ZB.Gameplay.PVP
{
    public partial class NetworkedPlayer : NetworkedEntity
    {
        #region Class Methods

        public virtual void RegisterGiveToken()
        {
            DieEvent += OnDie_CallbackOnGiveToken;
        }

        protected virtual void OnDie_CallbackOnGiveToken() { }

        #endregion Class Methods
    }
}