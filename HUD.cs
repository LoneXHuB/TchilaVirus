using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoneX.TchilaVirus
{
    public class HUD : MonoBehaviour
    {
        public static HUD instance;
        public PickableHudSlot[] pickableHudSlots;

        private void Awake()
        {
            if(instance == null)
                instance = this;
            else
                Destroy(this);
        }
        public void UpdatePickedItems(List<Pickable> _PickedItems)
        {
            for(int i = 0; i < _PickedItems.Count;i++)
            {
                pickableHudSlots[i].ActivateSlot(_PickedItems[i]);
            }
        } 
    }

}