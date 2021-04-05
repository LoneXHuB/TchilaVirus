using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace  LoneX.TchilaVirus
{
    public class Spawner : MonoBehaviourPunCallbacks
    {
        public static Spawner instance;
        public GameObject virusSpawnPoints;
        public GameObject WhiteSpawnPoints;

        private int virusCount;
        private int whiteCount;

        public void Awake()
        {
            virusCount = 0;
            whiteCount = 0;

            if(instance != null)
                Destroy(this);
            else
                instance = this;
        }

        public GameObject SpawnPlayer(Team _team)
        {
            Vector3 _spawnPosition = new Vector3();
            GameObject _myplayer;

            if(_team == Team.Virus)
            {
                _spawnPosition = virusSpawnPoints.transform.GetChild(virusCount).position;
                _myplayer = PhotonNetwork.Instantiate("Virus", _spawnPosition , Quaternion.identity, 0);
                virusCount++;
            }
            else
            {
                _spawnPosition = WhiteSpawnPoints.transform.GetChild(whiteCount).position;
                _myplayer = PhotonNetwork.Instantiate("WhiteCell", _spawnPosition , Quaternion.identity, 0);
                whiteCount++;
            }

            return _myplayer;
        }

        public GameObject SpawnAI(Team _team)
        {
            Vector3 _spawnPosition = new Vector3();
            GameObject _npc;

            if(_team == Team.Virus)
            {
                _spawnPosition = virusSpawnPoints.transform.GetChild(virusCount).position;
                _npc = PhotonNetwork.Instantiate("AIVirus", _spawnPosition , Quaternion.identity, 0);
                virusCount++;
            }
            else
            {
                _spawnPosition = WhiteSpawnPoints.transform.GetChild(whiteCount).position;
                _npc = PhotonNetwork.Instantiate("AIWhite", _spawnPosition , Quaternion.identity, 0);
                whiteCount++;
            }

            return _npc;
        }
    }    
}