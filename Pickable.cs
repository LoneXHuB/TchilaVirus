using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace LoneX.TchilaVirus {
    
    public enum PickableType { Magnesium = 1 , Sulfur , Glucose , Proteine , Vitamin }

    public class Pickable : MonoBehaviourPun
    {
        public string Id { get; set; }
        public bool isPicked = false;
        public Sprite sprite;
        [SerializeField]
        public PickableType pickableType;
        public Collider2D pickableCollider;
        public Animator animator;
        public int reactivationTime;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>().sprite;
            animator = GetComponent<Animator>();
        }
        private void Awake()
        {
            if(string.IsNullOrEmpty(Id))
            {
                Id = "pickable" + this.gameObject.name;
                if(! CollectiblesManager.pickables.ContainsKey(this.Id))
                    CollectiblesManager.AddPickable(this , isPicked);
            }
        }
        private void OnTriggerEnter2D(Collider2D _other)
        {
            PlayerManager _picker = _other.GetComponent<PlayerManager>();
            if(_picker!=null)
            {
                if(_picker.abilities.Count < 2 && _picker.photonView.IsMine)
                {
                    setPicked(true);
                    _picker.AddAbility(this);
                }
            }
        }
        public void setPicked(bool _isPicked)
        {
            isPicked = _isPicked;
            pickableCollider.enabled = !isPicked;
            CollectiblesManager.UpdateItemState(this);
        }
        public void Picked()
        {
            if(isPicked)
            {
                //Debug.Log($"Pickable id : {Id} picked ! ");
                StartCoroutine(ReActivatePickableIn(reactivationTime));
            }

            pickableCollider.enabled = !isPicked;
            animator.SetBool("isPicked", isPicked);
        }
        private IEnumerator ReActivatePickableIn(int sec)
        {
            yield return new WaitForSeconds(sec);
            
            isPicked = false;
            pickableCollider.enabled = !isPicked;
            animator.SetBool("isPicked", isPicked);
            CollectiblesManager.UpdateItemState(this);
            //Debug.Log($"Regenarted pickable{this.Id}");
        }
    }
}