using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LoneX.TchilaVirus
{
    public class ListItem : MonoBehaviour
    {
        #region PublicAtt
        public TMP_Text host;
        public TMP_Text mapName;
        public TMP_Text playerCount;
        public TMP_Text maxPlayers;
        public TMP_Text isPrivate;
        public static ListItem selectedItem;
        #endregion

        #region PrivateAtt
        #endregion

        #region MonoBehaviourCallbacks
        #endregion

        #region OtherCallbacks
        #endregion

        #region PublicMeths
        public void OnItemSelected()
        {
            selectedItem = this;
        }
        public void InitItem(string _host, string _mapName , string _playerCount , string _maxPlayers , bool _isPrivate)
        {
            host.text = _host;
            mapName.text = _mapName;
            playerCount.text = _playerCount;
            maxPlayers.text = _maxPlayers;
            if(_isPrivate)
                isPrivate.text = "YES";
            else
                isPrivate.text = "NO";

        }
        #endregion

        #region PrivateMeths
        #endregion
        
    }
}
