using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;
    public Text score;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists ! ");
            Destroy(this);
        }
    }

    public void SetNewScore(int _virusScore , int _whiteScore)
    {
        score.text = $"{_virusScore} - {_whiteScore}";
    }

}
