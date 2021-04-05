using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoneX.TchilaVirus
{
    public class PickableHudSlot : MonoBehaviour
    {
        public Image image;
        public Button button;
        public Pickable item;
        
        public static event EventHandler Clicked;

        void Start()
        {
            image.gameObject.SetActive(false);
        }

        public void SlotClicked()
        {
            if(item != null)
            {
                OnClicked();
            }
        }
        
        public void ActivateSlot(Pickable _pickable)
        {
            item = _pickable;
            image.gameObject.SetActive(true);
            image.sprite = _pickable.sprite;
        }

        protected virtual void OnClicked()
        {
            if(Clicked != null)
            Clicked(this, EventArgs.Empty);
        }

        public void EmptySlot()
        {
            image.gameObject.SetActive(false);
            image.sprite = null;
            item = null;
        }
    }

}