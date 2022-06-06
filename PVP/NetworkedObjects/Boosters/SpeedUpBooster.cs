using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class SpeedUpBooster : PermanentBooster
    {
        #region Membes

        [SerializeField]
        private float _bonusSpeedValue = 0.5f;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            networkedPlayer.AddSpeedValue(_bonusSpeedValue);
        }

        #endregion Class Methods
    }
}