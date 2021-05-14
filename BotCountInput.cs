using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
namespace LoneX.TchilaVirus
{
    public class BotCountInput : MonoBehaviour
    {
        const string BotCountPrefKey = "BotCount";
        public static int BotCount = 0;

        public void SetBotCount( string _botCount )
        {
            //Debug.Log($"BotCount will be {_botCount}");
            if (string.IsNullOrEmpty(_botCount))
            {
                //Debug.Log("BotCount null or empty");
                return;
            }

            BotCount =Int32.Parse(_botCount);
            
            //Debug.Log($"BotCount  : {_botCount}");

            PlayerPrefs.SetString(BotCountPrefKey, _botCount);
        }

        private void Start()
        {
            string _oldBotCount = string.Empty;
            TMP_InputField _inputfield = this.GetComponent<TMP_InputField>();
            
            if (_inputfield!=null)
            {
                if(PlayerPrefs.HasKey(BotCountPrefKey))
                {
                    _oldBotCount = PlayerPrefs.GetString(BotCountPrefKey);
                    _inputfield.text = _oldBotCount;
                }
            }

            BotCount = Int32.Parse(_oldBotCount);
        }
    }
}
