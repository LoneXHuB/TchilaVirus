using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace LoneX.TchilaVirus
{
    public class UIMap : MonoBehaviour
    {
        #region PublicAtt 
            public Animator animator;
            public string mapName;
        #endregion

        #region PrivateAtt
        #endregion

        #region MonoBehaviourCallbacks
        #endregion

        #region OtherCallbacks
        #endregion

        #region PublicMeths
            public void ShowFromTheLeft()
            {
                animator.SetTrigger("ShowFromLeft");
            }
            public void ShowFromTheRight()
            {
                animator.SetTrigger("ShowFromRight");
            }
            public void HidetoTheLeft()
            {
                animator.SetTrigger("HideToLeft");
            }
            public void HideToTheRight()
            {
                animator.SetTrigger("HideToRight");
            }
        #endregion

        #region PrivateMeths
        #endregion
        
    }
}
