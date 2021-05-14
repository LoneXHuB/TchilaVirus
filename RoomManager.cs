using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace LoneX.TchilaVirus
{
    public enum Team { White =  1 , Virus }

    public class RoomManager : MonoBehaviourPunCallbacks
    {
        #region public fields
        public static RoomManager instance;
        public Transform[] spawnPoints;

        public GameObject myplayer;
        #endregion
        
        #region private fields
        #endregion

        #region Photon Callbacks
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Monobehaviour callbacks
        private void Start()
        {
                //Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name);
            //we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            Team team;
            if(PhotonNetwork.LocalPlayer != null)
            team = (Team)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            else return;
            
            myplayer = Spawner.instance.SpawnPlayer(team);
            if(PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < BotCountInput.BotCount; i++)
                {
                    //Debug.Log("Spawning Bot");
                    if(i % 2 == 0)
                        Spawner.instance.SpawnAI(Team.Virus);
                    else
                        Spawner.instance.SpawnAI(Team.White);
                }
            }
        }

        private void Awake()
        {
            if(instance == null)
                instance = this;
            else
                Destroy(instance);
                
        }
        #endregion

        #region Public Methods
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void Respawn()
        {  
            PhotonNetwork.Destroy(myplayer);
            Team team = (Team)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

            myplayer = Spawner.instance.SpawnPlayer(team);
            
            //Debug.Log($"respawned {team} player!");
        }
        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                //Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            
            //Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for 1");
        }

        #endregion
    }

}
