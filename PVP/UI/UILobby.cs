using UnityEngine;
using UnityEngine.UI;

namespace ZB.Gameplay.PVP
{
    public class UILobby : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private string _connectedNetworkAddress = "103.167.198.128";
        [SerializeField]
        private GameObject _loadingPanel;
        [SerializeField]
        private GameObject _connectionErrorPanel;
        [SerializeField]
        private InputField _nameField;
        [SerializeField]
        private InputField _ipAddressField;
        [SerializeField]
        private Button _findMatchButton;
        [SerializeField]
        private Button[] _characterSelectionButtons;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _nameField.onValueChanged.AddListener(OnPlayerNameChanged);
            _ipAddressField.onValueChanged.AddListener(OnIPAddressChanged);
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(PrefsKeys.playerCharacterID))
                PlayerPrefs.SetInt(PrefsKeys.playerCharacterID, 0);

            if (!PlayerPrefs.HasKey(PrefsKeys.playerName))
                PlayerPrefs.SetString(PrefsKeys.playerName, "User" + System.String.Format("{0:0000}", Random.Range(1, 9999)));

            if (!PlayerPrefs.HasKey(PrefsKeys.serverAddress))
                PlayerPrefs.SetString(PrefsKeys.serverAddress, _connectedNetworkAddress);

            PlayerPrefs.Save();

            _nameField.text = PlayerPrefs.GetString(PrefsKeys.playerName);
            _ipAddressField.text = PlayerPrefs.GetString(PrefsKeys.serverAddress);

            int selectedCharacterID = PlayerPrefs.GetInt(PrefsKeys.playerCharacterID);
            for (int i = 0; i < _characterSelectionButtons.Length; i++)
            {
                if (selectedCharacterID == i)
                    _characterSelectionButtons[i].Select();

                int index = i;
                _characterSelectionButtons[i].onClick.AddListener(() => SelectPlayerCharacter(index));
            }

            CustomNetworkManager.singleton.networkAddress = _ipAddressField.text;
        }

        private void OnEnable()
        {
            CustomNetworkManager.OnClientConnected += HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected += HandleClientDisconnected;
            CustomNetworkManager.OnClientConnectionError += HandleClientConnectionError;
        }

        private void OnDisable()
        {
            CustomNetworkManager.OnClientConnected -= HandleClientConnected;
            CustomNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
            CustomNetworkManager.OnClientConnectionError -= HandleClientConnectionError;
        }

        #endregion API Methods

        #region Class Methods

        public void FindMatch()
        {
            _loadingPanel.SetActive(true);
            StartCoroutine(CustomNetworkManager.FindMatch());
        }

        private void OnPlayerNameChanged(string changedValue)
        {
            PlayerPrefs.SetString(PrefsKeys.playerName, changedValue);
            PlayerPrefs.Save();
        }

        private void OnIPAddressChanged(string changedValue)
        {
            PlayerPrefs.SetString(PrefsKeys.serverAddress, changedValue);
            PlayerPrefs.Save();
            CustomNetworkManager.singleton.networkAddress = changedValue;
        }

        private void SelectPlayerCharacter(int id)
        {
            PlayerPrefs.SetInt(PrefsKeys.playerCharacterID, id);
        }

        private void OnConnectionError()
        {
            _loadingPanel.SetActive(false);
            _connectionErrorPanel.SetActive(true);
        }

        private void HandleClientConnected()
        {
            _findMatchButton.interactable = true;
        }

        private void HandleClientDisconnected()
        {
            _findMatchButton.interactable = true;
        }

        private void HandleClientConnectionError()
        {
            OnConnectionError();
        }
    }

    #endregion Class Methods
}