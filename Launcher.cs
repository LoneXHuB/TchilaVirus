using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;
using System.Collections.Generic;

namespace LoneX.TchilaVirus
{
    public enum GameMode {Solo = 1 , QuickMatch , Host , Join}
    public class Launcher : MonoBehaviourPunCallbacks
    {
        string gameVersion = "1";

        private bool isConnecting;

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 10;
        public List<ListItem> items;
        public Toggle isPrivateRoom;
        private GameMode gameMode;
        public ListItem ItemTemplate;
        public Transform scrollViewContent;
        public TMP_InputField maxPlayerInput;
        public TMP_InputField botCountInput;
        public MessageBox messageBox;
        public GameObject connectingPanel;
        public Animator logoAnimator;
        public Animator exitAnimator;
        public Animator virusAnimator;
        public Animator mainMenuAnimator;
        public Animator whiteAnimator;
        public Animator onlineMenuAnimator;
        public GameObject teamSelectionPanelQG;
        public const string MAP_NAME_KEY = "mapName";
        public const string BOT_COUNT_KEY = "botCount";
        public const string TEAM_KEY = "team";
        
        
        
        #region MonoBehaviourCallbacks
            private void Awake()
            {
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        #endregion
        
        #region PublicMethods
            public void Connect()
            {
                connectingPanel.SetActive(true);
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

            public void QuickMatch(int team)
            {
                gameMode = GameMode.QuickMatch;

                Hashtable teamProp = new Hashtable
                    {
                        {TEAM_KEY, (Team)team }
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
                    Connect();
                }
            }
            public void JoinRoomMenu()
            {
                gameMode = GameMode.Join;

                if (PhotonNetwork.IsConnected)
                {
                    // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                    PhotonNetwork.JoinLobby();
                }
                else
                {
                    // #Critical, we must first and foremost connect to Photon Online Server.
                    Connect();
                }
            }
            public void CreateGame(int _team)
            {
                gameMode = GameMode.Host;

                Hashtable teamProp = new Hashtable
                {
                    {TEAM_KEY, (Team)_team }
                };
                
                PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

                RoomOptions _roomOptions = new RoomOptions();
                _roomOptions.CustomRoomPropertiesForLobby = new string[] {MAP_NAME_KEY};
                try
                {
                    _roomOptions.MaxPlayers = Byte.Parse(maxPlayerInput.text);
                    _roomOptions.CustomRoomProperties = new Hashtable() { {BOT_COUNT_KEY, Byte.Parse(botCountInput.text)} , {MAP_NAME_KEY , "Room for 1"}};
                }
                catch(Exception e) { ShowMessage("Woops ! " , e.Message); return;}
                
                
                if(PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.UserId , _roomOptions);
                }
                else
                {
                    // #Critical, we must first and foremost connect to Photon Online Server.
                    Connect();
                }
            }

            public void ReturnToMainMenu()
            {
                GameObject[] _menus = GameObject.FindGameObjectsWithTag("Menu");

                foreach(GameObject _menu in _menus)
                {
                    Animator _menuAnimator = _menu.GetComponent<Animator>();
                    if(_menuAnimator != null)
                    {
                        if(_menuAnimator != mainMenuAnimator && _menuAnimator !=  logoAnimator && _menuAnimator != exitAnimator && _menuAnimator != virusAnimator)
                            _menuAnimator.SetTrigger("Hide");
                        else
                            _menuAnimator.SetTrigger("Show");

                    }
                }
            }

            public void ShowMessage(string _title , string _message)
            {
                messageBox.gameObject.SetActive(true);
                messageBox.Title = _title;
                messageBox.Message = _message;
            }
        #endregion

        #region MonoBehaviourPunCallbacks callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log($"Connected to Pun Server : {PhotonNetwork.ServerAddress}");
            connectingPanel.SetActive(false);
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            if (isConnecting)
            {
                switch (gameMode)
                {
                    case GameMode.Host :
                        Team _team = (Team) PhotonNetwork.LocalPlayer.CustomProperties[TEAM_KEY];
                        CreateGame((int) _team);
                        break;
                    case GameMode.QuickMatch :
                        PhotonNetwork.JoinRandomRoom();
                        break;
                    case GameMode.Join :
                        PhotonNetwork.JoinLobby();
                        break;
                    default:
                    break;
                }
                isConnecting = false;
            }
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Join failed, creating room...");
            ShowMessage("Oops !" , returnCode +"\n"+ message);
            connectingPanel.SetActive(false);
            teamSelectionPanelQG.GetComponent<Animator>().SetTrigger("Hide");
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            ShowMessage("Woops !" , returnCode +"\n"+ message);
            ReturnToMainMenu();
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("room joined successfully");

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                
                string _mapName = PhotonNetwork.CurrentRoom.CustomProperties[MAP_NAME_KEY].ToString();
                Debug.Log("Loading" + _mapName);

                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel(_mapName);
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat($"Disconnected from pun server, cause : {cause}");
            
            connectingPanel.gameObject.SetActive(false);

            ShowMessage("Disconnected" , $"{cause}");

            ReturnToMainMenu();
            
            isConnecting = false;
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach(RoomInfo _room in roomList)
            {
                if(_room.IsOpen == isPrivateRoom.isOn)
                    continue;

                GameObject _itemGameObject = Instantiate(ItemTemplate.gameObject , scrollViewContent);
                _itemGameObject.GetComponent<ListItem>();

                if(_room.RemovedFromList)
                {
                    int _index = items.FindIndex(x => x.host.text == _room.Name);
                    if(_index != -1)
                    {
                        Destroy(items[_index].gameObject);
                        continue;
                    }
                }

                ListItem _item = _itemGameObject.GetComponent<ListItem>();
                _item.InitItem(_room.Name , _room.CustomProperties[MAP_NAME_KEY].ToString() , _room.PlayerCount.ToString() , _room.MaxPlayers.ToString() , _room.IsOpen);
                items.Add(_item);
            }
        }
        #endregion
    }

}