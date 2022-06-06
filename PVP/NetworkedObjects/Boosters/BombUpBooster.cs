using UnityEngine;

namespace ZB.Gameplay.PVP
{
    public class BombUpBooster : PermanentBooster
    {
        #region Membes

        [SerializeField]
        private int _bonusBombNumber = 1;

        #endregion Members

        #region Class Methods

        public override void GetCollected(NetworkedPlayer networkedPlayer)
        {
            networkedPlayer.AddBombNumber(_bonusBombNumber);
        }

        #endregion Class Methods
    }
}