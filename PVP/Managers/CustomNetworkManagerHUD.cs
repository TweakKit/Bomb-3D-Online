using UnityEngine;
using Mirror;

namespace ZB.Gameplay.PVP
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CustomNetworkManager))]
    public class CustomNetworkManagerHUD : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private int _offsetX = 5;
        [SerializeField]
        private int _offsetY = 50;
        [SerializeField]
        private int _HUDWidth = 215;
        [SerializeField]
        private int _HUDHeight = 9999;
        private GUIStyle _style;

        #endregion Members

        #region Properties

        #endregion Properties

        private CustomNetworkManager CustomNetworkManager
        {
            get
            {
                return NetworkManager.singleton as CustomNetworkManager;
            }
        }

        #region API Methods

        private void Start()
        {
            _style = new GUIStyle();
            _style.normal.textColor = Color.red;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(_offsetX, Screen.height - _offsetY, _HUDWidth, _HUDHeight));

            if (!NetworkClient.isConnected && !NetworkServer.active)
                StartButtons();
            else
                StatusLabels();

            GUILayout.EndArea();
        }

        private void StartButtons()
        {
            if (!NetworkClient.active)
            {
                if (GUILayout.Button("Host (Server + Client)"))
                    CustomNetworkManager.StartHost();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                    CustomNetworkManager.StartClient();

                CustomNetworkManager.networkAddress = GUILayout.TextField(CustomNetworkManager.networkAddress);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label($"Connecting to {CustomNetworkManager.networkAddress}..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                    CustomNetworkManager.StopClient();
            }
        }

        private void StatusLabels()
        {
            if (NetworkServer.active && NetworkClient.active)
                GUILayout.Label($"Host: running via {Transport.activeTransport}", _style);
            else if (NetworkServer.active)
                GUILayout.Label($"Server: running via {Transport.activeTransport}", _style);
            else if (NetworkClient.isConnected)
                GUILayout.Label($"Client: connected to {CustomNetworkManager.networkAddress} via {Transport.activeTransport}", _style);
        }

        #endregion API Methods
    }
}