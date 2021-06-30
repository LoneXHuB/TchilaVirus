using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LoneX.TchilaVirus
{
    public class MessageBox : MonoBehaviour
    {
        #region PublicAtt
        [HideInInspector]
        public string Title{ set{titleTextField.text = value;} }
        [HideInInspector]
        public string Message{ set{messageTextField.text = value;} }
        public TMP_Text titleTextField;
        public TMP_Text messageTextField;
        #endregion

        #region PrivateAtt
        #endregion

        #region MonoBehaviourCallbacks
        #endregion

        #region OtherCallbacks
        #endregion

        #region PublicMeths
        public void ButtonClosed()
        {
            this.gameObject.SetActive(false);
        }
        #endregion

        #region PrivateMeths
        #endregion
        
    }
}
