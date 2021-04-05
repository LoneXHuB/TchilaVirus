using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace LoneX.TchilaVirus
{
public class GameManager : MonoBehaviourPunCallbacks
{
    #region fields
        private int shieldCount = 0;
        public TMP_Text scoreText;
        public TMP_Text CenterText;
        public float maxGameTimer;
        public float gameTimer;
        public int score;
        public static GameManager instance;
        public static bool isGameover = false;

    #endregion

    #region MonoBehaviour Callbacks
        private void Start()
        {
            InitializeRoomProperties();
        }
        private void Awake()
        {
            if(instance == null)
                instance = this;
            else
                Destroy(this);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            CenterText.gameObject.SetActive(false);
         
            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerHasExpired;
         
            isGameover= false;
         
            if(PhotonNetwork.IsMasterClient)
            {
                CountdownTimer.SetStartTime();
            }
        }

         public override void OnDisable()
        {
            base.OnDisable();
            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerHasExpired;
        }
    #endregion
    

    #region CountDownTimer Callbacks
        public void OnCountdownTimerHasExpired()
        {
            if(!PhotonNetwork.InRoom)
            return;

            DisplayGameOverText();
        }
        #endregion

        #region Photon Callbacks
       
        public override void OnJoinedRoom()
        {
            
            Debug.Log("OnJoinedRoom called in Game Manager");
            if(PhotonNetwork.CurrentRoom.CustomProperties["ShieldCount"] != null 
            && PhotonNetwork.CurrentRoom.CustomProperties["gameTimer"] != null
            && PhotonNetwork.CurrentRoom.CustomProperties["score"] != null)
            {
                gameTimer = float.Parse(PhotonNetwork.CurrentRoom.CustomProperties["gameTimer"].ToString());
            
                shieldCount = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["ShieldCount"].ToString());

                score = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["score"].ToString());
                scoreText.text = $"{score}/{shieldCount}";
            }else
                RoomManager.instance.LeaveRoom();
            
        }

        public override void  OnRoomPropertiesUpdate(Hashtable _propertiesThatChanged)
        {
            if( _propertiesThatChanged["score"] != null && _propertiesThatChanged["ShieldCount"] != null)
            {
                score = int.Parse(_propertiesThatChanged["score"].ToString());
                shieldCount = int.Parse(_propertiesThatChanged["ShieldCount"].ToString());
                scoreText.text = $"{score}/{shieldCount}";
            }
        }
    #endregion

    #region LocalMethods
        public void DisplayGameOverText()
        {
            isGameover = true;
            CenterText.gameObject.SetActive(true);
            
            PlayerManager _player = RoomManager.instance.myplayer.GetComponent<PlayerManager>();

            if(PlayerWon(_player))
                CenterText.text = "Mission Acomplished !";
            else
                CenterText.text = "Mission Failed !";
        }

        public bool PlayerWon(PlayerManager _player)
        {
            if(_player.team == Team.Virus)
                if(score < shieldCount)
                {
                   return false;
                }
                else 
                {
                   return true;
                }
            else 
                if(score < shieldCount)
                {
                   return true;
                }
                else 
                {
                   return false;
                }
        }
        public void InitializeRoomProperties()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                gameTimer = maxGameTimer;
                shieldCount = ShieldsManager.instance.transform.childCount;
                Hashtable _timeProp = new Hashtable {{"gameTimer", gameTimer} , {"ShieldCount" , shieldCount},{"score" , score}};
                
                InitScoreText();

                PhotonNetwork.CurrentRoom.SetCustomProperties(_timeProp);
            }
        }
        public void UpdateScore()
        {
            if(!isGameover)
            {    
                score++;
                scoreText.text = $"{score}/{shieldCount}";
                Hashtable _scoreProp = new Hashtable {{"score", score}};
                
                if(score >= shieldCount)
                    DisplayGameOverText();

                PhotonNetwork.CurrentRoom.SetCustomProperties(_scoreProp);
            }
        }
        public void InitScoreText()
        {
            if(scoreText != null)
                scoreText.text = $"{score}/{shieldCount}";
        }
        
    #endregion


}

}