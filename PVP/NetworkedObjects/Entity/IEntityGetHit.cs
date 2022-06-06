using Mirror;

namespace ZB.Gameplay.PVP
{
    public interface IEntityGetHit
    {
        #region Interface Methods

        [Server]
        void GetHit(float damageValue);

        #endregion Interface Methods
    }
}