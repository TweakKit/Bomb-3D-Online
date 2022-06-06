using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace ZB.Gameplay.PVP
{
    public class NetworkRoomPlayer : NetworkBehaviour
    {
        #region Members

        [HideInInspector]
        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string displayName = "";

        [HideInInspector]
        [SyncVar(hook = nameof(HandleAvatarIndexChanged))]
        public int avatarIndex;

        [HideInInspector]
        [SyncVar(hook = nameof(HandleIsReadyChanged))]
        public bool isReady = false;

        [SerializeField]
        private GameObject _panelLobbyUI;
        [SerializeField]
        private GameObject _waitQueuePanelsContainer;
        [SerializeField]
        private GameObject[] _waitQueuePanels;
        [SerializeField]
        private TextMeshProUGUI _readyText;
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private Sprite _readySprite;
        [SerializeField]
        private Sprite _unreadySprite;
        [SerializeField]
        private Sprite[] _playerAvatarSprites;

        [SyncVar]
        private bool _isLeader = false;
        private JoinMessage _joinMessage;
        private CustomNetworkManager _customNetworkManager;

        #endregion Members

        #region Properties

        public bool IsLeader
        {
            get
            {
                return _isLeader;
            }
            set
            {
                _isLeader = value;
            }
        }

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
                if (_customNetworkManager != null)
                    return _customNetworkManager;

                return _customNetworkManager = NetworkManager.singleton as CustomNetworkManager;
            }
        }

        #endregion Properties

        #region API Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            _waitQueuePanels = new GameObject[_waitQueuePanelsContainer.transform.childCount];
            for (int i = 0; i < _waitQueuePanelsContainer.transform.childCount; i++)
                _waitQueuePanels[i] = _waitQueuePanelsContainer.transform.GetChild(i).gameObject;
        }
#endif

        #endregion API Methods

        #region Class Methods

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerPrefs.GetString(PrefsKeys.playerName));
            CmdSetAvatar(PlayerPrefs.GetInt(PrefsKeys.playerCharacterID));
            _panelLobbyUI.SetActive(true);
            _startGameButton.gameObject.SetActive(_isLeader);
        }

#if !UNITY_EDITOR
        public override void OnStartServer()
        {
            CustomNetworkManager.NetworkRoomPlayers.Add(this);
        }
#endif

        public override void OnStartClient()
        {
            CustomNetworkManager.NetworkRoomPlayers.Add(this);
            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            CustomNetworkManager.NetworkRoomPlayers.Remove(this);
            UpdateDisplay();
        }

        [ClientRpc]
        public void HandleReadyToStart(bool readyToStart)
        {
            if (_isLeader)
                _startGameButton.interactable = readyToStart;
        }

        public void Ready()
        {
            _readyText.text = !isReady ? "Unready" : "Ready";
            CmdReady();
        }

        public void StartGame()
        {
            CmdStartGame();
        }

        private void UpdateDisplay()
        {
            if (hasAuthority)
            {
                for (int i = 0; i < CustomNetworkManager.NetworkRoomPlayers.Count; i++)
                {
                    _waitQueuePanels[i].SetActive(true);
                    _waitQueuePanels[i].GetComponent<Image>().sprite = CustomNetworkManager.NetworkRoomPlayers[i].isReady ? _readySprite : _unreadySprite;
                    _waitQueuePanels[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = CustomNetworkManager.NetworkRoomPlayers[i].displayName;
                    _waitQueuePanels[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = CustomNetworkManager.NetworkRoomPlayers[i].isReady ? "<color=red>Ready</color>" : "<color=yellow>Not Ready</color>";
                    _waitQueuePanels[i].transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _playerAvatarSprites[CustomNetworkManager.NetworkRoomPlayers[i].avatarIndex];
                }

                for (int i = CustomNetworkManager.NetworkRoomPlayers.Count; i < _waitQueuePanels.Length; i++)
                    _waitQueuePanels[i].SetActive(false);
            }
            else
            {
                foreach (var player in CustomNetworkManager.NetworkRoomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }
            }
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        [Command]
        private void CmdSetAvatar(int avatarIndex)
        {
            this.avatarIndex = avatarIndex;
        }

        [Command]
        private void CmdReady()
        {
            isReady = !isReady;
            CustomNetworkManager.NotifyPlayersOfReadyState();
        }

        [Command]
        private void CmdStartGame()
        {
            if (CustomNetworkManager.NetworkRoomPlayers[0].connectionToClient != connectionToClient)
                return;

            CustomNetworkManager.StartGame();
        }

        private void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
        private void HandleAvatarIndexChanged(int oldValue, int newValue) => UpdateDisplay();
        private void HandleIsReadyChanged(bool oldValue, bool newValue) => UpdateDisplay();

        #endregion Class Methods
    }
}