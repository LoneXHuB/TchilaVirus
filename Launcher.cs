using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

namespace LoneX.TchilaVirus
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1";

        private bool isConnecting;

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [SerializeField]
        private GameObject LoadingPanel;

        [SerializeField]
        private GameObject ControlPanel;


        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            ControlPanel.SetActive(true);
            LoadingPanel.SetActive(false);
        }

        public void Connect(int team)
        {
            ControlPanel.SetActive(false);
            LoadingPanel.SetActive(true);

            Hashtable teamProp = new Hashtable
                {
                    {"Team" , (Team)team }
                };

            PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #region MonoBehaviourPunCallbacks callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log($"Connected to Pun Server : {PhotonNetwork.ServerAddress}");
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Join failed, creating room...");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            SceneManager.LoadScene(0);
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("room joined successfully");

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("Loading room for 1");

                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat($"Disconnected from pun server, cause : {cause}");

            ControlPanel.SetActive(true);
            LoadingPanel.SetActive(false);

            isConnecting = false;
        }
        #endregion
    }

}