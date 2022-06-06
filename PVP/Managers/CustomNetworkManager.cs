using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class CustomNetworkManager : NetworkManager
    {
        #region Members

        [Header("Registered network prefabs")]

        [SerializeField]
        private List<GameObject> _registeredNetworkScenePrefabs = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _registeredNetworkMapTilePrefabs = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _registeredNetworkPlayerPrefabs = new List<GameObject>();
        [SerializeField]
        private List<GameObject> _registeredNetworkBoosterPrefabs = new List<GameObject>();

        [Header("--- Lobby Scene ---")]

        [SerializeField]
        [Scene]
        private string _lobbyScene;
        [SerializeField]
        private NetworkRoomPlayer _networkRoomPlayerPrefab;
        [SerializeField]
        private int _minPlayers = 2;

        [Header("--- Match Scene ---")]

        [SerializeField]
        [Scene]
        private string _matchScene;
        [SerializeField]
        private NetworkMatchPlayer _networkMatchPlayerPrefab;
        [SerializeField]
        private NetworkDataManager _networkDataManagerPrefab;
        [SerializeField]
        private NetworkLevelLoader _networkLevelLoaderPrefab;
        [SerializeField]
        private PlayerSpawnerManager _playerSpawnerManagerPrefab;
        [SerializeField]
        private MatchStarterManager _matchStarterManagerPrefab;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action OnClientConnectionError;
        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;

        #endregion Members

        #region Properties

        public List<NetworkRoomPlayer> NetworkRoomPlayers { get; } = new List<NetworkRoomPlayer>();
        public List<NetworkMatchPlayer> NetworkMatchPlayers { get; } = new List<NetworkMatchPlayer>();

        #endregion Properties

        #region Class Methods

        protected override void RegisterClientMessages()
        {
            base.RegisterClientMessages();

            foreach (GameObject prefab in _registeredNetworkScenePrefabs.Where(t => t != null))
                NetworkClient.RegisterPrefab(prefab);

            foreach (GameObject prefab in _registeredNetworkMapTilePrefabs.Where(t => t != null))
                NetworkClient.RegisterPrefab(prefab);

            foreach (GameObject prefab in _registeredNetworkPlayerPrefabs.Where(t => t != null))
                NetworkClient.RegisterPrefab(prefab);

            foreach (GameObject prefab in _registeredNetworkBoosterPrefabs.Where(t => t != null))
                NetworkClient.RegisterPrefab(prefab);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<JoinMessage>(OnServerAddPlayer);
        }

        public override void OnStopServer()
        {
            OnServerStopped?.Invoke();
            NetworkRoomPlayers.Clear();
            NetworkMatchPlayers.Clear();
            NetworkServer.UnregisterHandler<JoinMessage>();
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            OnServerReadied?.Invoke(conn);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
        }

        /// <summary>
        /// Override for the callback received on the server when a client disconnects from the game.
        /// Updates the game UI to correctly display the decreased team size. This is not called for
        /// the server itself, thus the workaround in GameManager's OnHostMigration method is needed.
        /// </summary>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn == null || conn.identity == null)
                return;

            var networkRoomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();
            if (networkRoomPlayer != null)
            {
                NetworkRoomPlayers.Remove(networkRoomPlayer);
                NotifyPlayersOfReadyState();
            }
            else
            {
                var networkMatchPlayer = conn.identity.GetComponent<NetworkMatchPlayer>();
                if (networkMatchPlayer != null)
                    NetworkMatchPlayers.Remove(networkMatchPlayer);
            }

            base.OnServerDisconnect(conn);
        }

        /// <summary>
        /// Override for callback received (on the client) when joining a game.
        /// Same as in the UNET source, but modified AddPlayer method with more parameters.
        /// </summary>
        public override void OnClientConnect()
        {
            if (!clientLoadedScene)
            {
                OnClientConnected?.Invoke();
                NetworkClient.Ready();
                if (autoCreatePlayer && NetworkClient.localPlayer == null)
                    NetworkClient.connection.Send(GetJoinMessage());
            }
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnected?.Invoke();
        }

        public override void OnClientError(Exception exception)
        {
            base.OnClientError(exception);
            OnClientConnectionError?.Invoke();
        }

        public override void ServerChangeScene(string newSceneName)
        {
            for (int i = NetworkRoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = NetworkRoomPlayers[i].connectionToClient;
                var networkMatchPlayer = Instantiate(_networkMatchPlayerPrefab);
                networkMatchPlayer.JoinMessage = NetworkRoomPlayers[i].JoinMessage;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, networkMatchPlayer.gameObject);
            }

            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            NetworkDataManager networkDataManager = Instantiate(_networkDataManagerPrefab);
            NetworkServer.Spawn(networkDataManager.gameObject);

            NetworkLevelLoader networkLevelLoader = Instantiate(_networkLevelLoaderPrefab);
            NetworkServer.Spawn(networkLevelLoader.gameObject);

            PlayerSpawnerManager playerSpawnerManager = Instantiate(_playerSpawnerManagerPrefab);
            NetworkServer.Spawn(playerSpawnerManager.gameObject);

            MatchStarterManager matchStarterManager = Instantiate(_matchStarterManagerPrefab);
            NetworkServer.Spawn(matchStarterManager.gameObject);
        }

        /// <summary>
        /// Override for the callback received when a client disconnected.
        /// Eventual cleanup of internal high level API UNET variables.
        /// </summary>
        public override void OnStopClient()
        {
            // Because we are not using the automatic scene switching and cleanup by Unity Networking, the current network scene is still set to the online scene
            // even after disconnecting. So to clean that up for internal reasons, we simply set it to an empty string here.
            networkSceneName = "";
        }

        /// <summary>
        /// Start finding a match.
        /// </summary>
        public static IEnumerator FindMatch()
        {
            yield return null;
            singleton.StartClient();
        }

        public void NotifyPlayersOfReadyState()
        {
            NetworkRoomPlayers.ForEach(x => x.HandleReadyToStart(IsReadyToStart()));
        }

        public void StartGame()
        {
            if (!IsReadyToStart())
                return;

            ServerChangeScene(_matchScene);
        }

        private void OnServerAddPlayer(NetworkConnectionToClient conn, JoinMessage message)
        {
            if (SceneManager.GetActiveScene().path.Equals(_lobbyScene))
            {
                NetworkRoomPlayer networkRoomPlayer = Instantiate(_networkRoomPlayerPrefab);
                networkRoomPlayer.IsLeader = NetworkRoomPlayers.Count == 0;
                networkRoomPlayer.JoinMessage = message;
                NetworkServer.AddPlayerForConnection(conn, networkRoomPlayer.gameObject);
            }
        }

        private JoinMessage GetJoinMessage()
        {
            JoinMessage message = new JoinMessage();
            message.prefabId = PlayerPrefs.GetInt(PrefsKeys.playerCharacterID);
            message.playerName = PlayerPrefs.GetString(PrefsKeys.playerName);
            return message;
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < _minPlayers)
                return false;

            if (NetworkRoomPlayers.Any(x => !x.isReady))
                return false;

            return true;
        }

        #endregion Class Methods
    }

    [Serializable]
    public struct JoinMessage : NetworkMessage
    {
        #region Members

        public int prefabId;
        public string playerName;

        #endregion Members
    }
}