using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkMatchPlayer : NetworkBehaviour
    {
        #region Members

        private JoinMessage _joinMessage;

        #endregion Members

        #region Properties

        public JoinMessage JoinMessage
        {
            get
            {
                return _joinMessage;
            }
            set
            {
                _joinMessage = value;
            }
        }

        private CustomNetworkManager CustomNetworkManager
        {
            get
            {
                return NetworkManager.singleton as CustomNetworkManager;
            }
        }

        #endregion Properties

        #region API Methods

#if !UNITY_EDITOR
        public override void OnStartServer()
        {
            gameObject.name = "Match Player " + _joinMessage.playerName;
            DontDestroyOnLoad(gameObject);
            CustomNetworkManager.NetworkMatchPlayers.Add(this);
        }
#endif
        public override void OnStartClient()
        {
            gameObject.name = "Match Player " + _joinMessage.playerName;
            DontDestroyOnLoad(gameObject);
            CustomNetworkManager.NetworkMatchPlayers.Add(this);
        }

        public override void OnStopClient()
        {
            CustomNetworkManager.NetworkMatchPlayers.Remove(this);
        }

        #endregion API Methods
    }
}