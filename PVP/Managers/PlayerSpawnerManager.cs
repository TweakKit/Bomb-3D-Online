using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class PlayerSpawnerManager : NetworkBehaviour
    {
        #region Members

        [SerializeField]
        private GameObject[] _playerPrefabs;
        private List<MapAnchor> _playerOccupiedSlots;

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
        private void OnDestroy()
        {
            _playerOccupiedSlots = null;
            CustomNetworkManager.OnServerReadied -= SpawnPlayer;
        }

        #endregion API Methods

        #region Class Methods

        public override void OnStartServer()
        {
            _playerOccupiedSlots = new List<MapAnchor>();
            CustomNetworkManager.OnServerReadied += SpawnPlayer;
        }

        [Server]
        public void SpawnPlayer(NetworkConnection conn)
        {
            JoinMessage joinMessage = CustomNetworkManager.NetworkMatchPlayers.Find(x => x.connectionToClient == conn).JoinMessage;
            int playerSelectedCharacterID = joinMessage.prefabId;
            string playerName = joinMessage.playerName;
            Vector3 playerPosition = MapManager.GetCharacterPosition(ref _playerOccupiedSlots);
            GameObject playerGameObject = Instantiate(_playerPrefabs[playerSelectedCharacterID], playerPosition, Quaternion.identity);
            NetworkedPlayer player = playerGameObject.GetComponent<NetworkedPlayer>();
            player.name = playerName;
            player.teamIndex = -1;
            NetworkServer.Spawn(playerGameObject, conn);
        }

        #endregion Class Methods
    }
}