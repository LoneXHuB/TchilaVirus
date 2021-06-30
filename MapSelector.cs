using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus
{
    public class MapSelector : MonoBehaviour
    {
        #region PublicAtt
        #endregion

        #region StaticFields
        public static UIMap SelectedMap;
        #endregion

        #region PrivateAtt
        #endregion

        #region MonoBehaviourCallbacks
        void Start()
        {
            SelectedMap = transform.GetChild(0).GetComponent<UIMap>();
            SelectedMap.ShowFromTheLeft();
        }
        #endregion

        #region OtherCallbacks
        public void NextMapButton_Click ()
        {
            int _nexMapIndex = SelectedMap.transform.GetSiblingIndex() + 1;
            if(transform.childCount - 1 >= _nexMapIndex)
            {
                UIMap _nextMap = this.transform.GetChild(_nexMapIndex).GetComponent<UIMap>();
                if(_nextMap != null)
                {
                    _nextMap.ShowFromTheRight();
                    SelectedMap.HidetoTheLeft();
                    SelectedMap = _nextMap;
                }
            }
        }

        public void PreviousMapButton_Click ()
        {
            int _previousMapIndex = SelectedMap.transform.GetSiblingIndex() - 1;
            if(_previousMapIndex >= 0)
            {
                UIMap _previousMap = this.transform.GetChild(_previousMapIndex).GetComponent<UIMap>();
                if(_previousMap != null)
                {
                    _previousMap.ShowFromTheLeft();
                    SelectedMap.HideToTheRight();
                    SelectedMap = _previousMap;
                }
            }
        }
        #endregion

        #region PublicMeths
        #endregion

        #region PrivateMeths
        #endregion
        
    }
}
