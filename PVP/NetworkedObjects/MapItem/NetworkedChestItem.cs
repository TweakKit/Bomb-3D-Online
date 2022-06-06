using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkedChestItem : NetworkedMapItem
    {
        #region Class Methods

        [Server]
        protected override void DestroySelf()
        {
            // TODO: Send token for players.
            base.DestroySelf();
        }

        #endregion Class Methods
    }
}