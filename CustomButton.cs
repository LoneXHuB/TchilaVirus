using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoneX.TchilaVirus 
{
    public class CustomButton : MonoBehaviour
    {
        #region PublicAtt
        #endregion

        #region PrivateAtt
        #endregion

        #region MonoBehaviourCallbacks
            // Start is called before the first frame update
            void Start()
            {
                this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
            }
        #endregion

        #region OtherCallbacks
        #endregion

        #region PublicMeths
        #endregion

        #region PrivateMeths
        #endregion
        
    }
}
