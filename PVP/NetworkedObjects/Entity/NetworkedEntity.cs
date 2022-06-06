using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public abstract class NetworkedEntity : NetworkBehaviour
    {
        #region Members

        [HideInInspector]
        [SyncVar]
        public new string name;

        [HideInInspector]
        [SyncVar]
        public bool isDead;

        #endregion Members
    }
}