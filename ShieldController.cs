using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LoneX.TchilaVirus
{
    public class ShieldController : MonoBehaviourPun
    {
        private Rigidbody2D rigid; 
        private Animator animator;
        public Animator minimapTagAnimator;

        public bool isActivated;
        private void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }
        private void Awake()
        {
            isActivated = false;
        }

        private void FixedUpdate()
        {
            if(photonView.IsMine || ! PhotonNetwork.IsConnected)
                rigid.rotation += 1f;
        }
        
        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.gameObject != null)
            {
                PlayerManager _player = _other.gameObject.GetComponentInParent<PlayerManager>();

                if (_player != null)
                {
                    //if player is whiteCell
                    if (_player.team == Team.White)
                    {
                        _player.ChangeHealthBy(-10f);
                    }

                    return;
                }
            }
        }

        [PunRPC]
        private void ActivateShield()
        {
            animator.SetTrigger("Activated");
            minimapTagAnimator.SetTrigger("Activated");
            Debug.Log("Shield Activated");
            isActivated = true;
            GameManager.instance.UpdateScore();

            int _index = this.transform.GetSiblingIndex();
            GeneralAI.activatedShields[GameManager.instance.score - 1] = _index;
        }
    }
}