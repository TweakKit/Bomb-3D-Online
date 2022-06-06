using System.Linq;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class MatchStarterManager : NetworkBehaviour
    {
        #region Members

        [SerializeField]
        private Animator _animator;

        #endregion Members

        #region Properties

        private CustomNetworkManager CustomNetworkManager
        {
            get
            {
                return NetworkManager.singleton as CustomNetworkManager;
            }
        }

        #endregion Properties

        #region API Methods

        [ServerCallback]
        private void OnDestroy() => CleanUpServer();

        #endregion API Methods

        #region Class Methods

        public override void OnStartServer()
        {
            CustomNetworkManager.OnServerReadied += CheckToStartMatch;
            CustomNetworkManager.OnServerStopped += CleanUpServer;
        }

        [ServerCallback]
        public void StartMatch()
        {
            RpcStartMatch();
        }

        [Server]
        private void CheckToStartMatch(NetworkConnection conn)
        {
            if (CustomNetworkManager.NetworkMatchPlayers.Count(x => x.connectionToClient.isReady) != CustomNetworkManager.NetworkMatchPlayers.Count)
                return;

            _animator.enabled = true;
            RpcStartCountdown();
        }

        public void CountdownEnded()
        {
            _animator.enabled = false;
        }

        [ClientRpc]
        private void RpcStartCountdown()
        {
            _animator.enabled = true;
        }

        [ClientRpc]
        private void RpcStartMatch() { }

        [Server]
        private void CleanUpServer()
        {
            CustomNetworkManager.OnServerReadied -= CheckToStartMatch;
            CustomNetworkManager.OnServerStopped -= CleanUpServer;
        }

        #endregion Class Methods
    }
}