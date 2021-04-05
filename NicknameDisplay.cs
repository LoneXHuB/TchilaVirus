using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class NicknameDisplay : MonoBehaviour
{
    public PhotonView photonView;
    public TMP_Text username;

    public static Vector2 myPosition;
    void Update()
    {
        Vector2 _playerPosition = photonView.transform.position;
        this.transform.position = _playerPosition + Vector2.up * 1.75f;
        this.transform.rotation = Quaternion.identity;
        if(photonView.IsMine)
        {
            myPosition = photonView.transform.position;
        }
        else
        {
            Vector2 _otherPosition = photonView.transform.position;
            float _distance = Vector2.Distance(myPosition , _otherPosition);
            
            float _opacity = CalculateOpacity(_distance);
            username.color = new Color(1f,1f,1f,_opacity);
        }
    }

    private float CalculateOpacity(float _distance)
    {
        _distance = Mathf.Clamp(_distance,10f,15f);

        float _a = -1/5f;
        float _b = 3f;

        return (_a * _distance + _b);
    }
}
