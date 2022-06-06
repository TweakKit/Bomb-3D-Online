using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class PowerUpBooster : PermanentBooster
    {
        #region Membes

        [SerializeField]
        private float _bonusDamageValue = 5;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            networkedPlayer.AddBombDamageValue(_bonusDamageValue);
        }

        #endregion Class Methods
    }
}