using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class UsernameInput : MonoBehaviour
{
    #region Constants
    const string playerNamePrefKey = "PlayerName";
    #endregion

    #region public methods

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            //Debug.Log("username null or empty");
            return;
        }

        PhotonNetwork.NickName = value;
        //Debug.Log($"username  : {value}");

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        string _oldUsername = string.Empty;
        TMP_InputField _inputfield = this.GetComponent<TMP_InputField>();
        
        if (_inputfield!=null)
        {
            if(PlayerPrefs.HasKey(playerNamePrefKey))
            {
                _oldUsername = PlayerPrefs.GetString(playerNamePrefKey);
                _inputfield.text = _oldUsername;
            }
        }

        PhotonNetwork.NickName = _oldUsername;
    }
    #endregion
}
