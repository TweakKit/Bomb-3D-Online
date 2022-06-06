using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class RangeUpBooster : PermanentBooster
    {
        #region Membes

        [SerializeField]
        private int _bonusRadiusValue = 1;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            networkedPlayer.AddBombDamageRadius(_bonusRadiusValue);
        }

        #endregion Class Methods
    }
}