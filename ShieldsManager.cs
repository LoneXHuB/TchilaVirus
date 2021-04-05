using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace LoneX.TchilaVirus
{
public class ShieldsManager : MonoBehaviourPun
{
    public static ShieldsManager instance;
    public int score;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void UpdateScore(int _score)
    {
        score = _score;
    }
}

}