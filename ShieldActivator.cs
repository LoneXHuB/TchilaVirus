using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LoneX.TchilaVirus
{
    public class ShieldActivator : MonoBehaviour
    {
            private Animator animator;
            
            private void Start()
            {
                animator = GetComponentInParent<Animator>();
            }
            private void OnTriggerEnter2D(Collider2D _other)
            {
                if (_other.gameObject != null)
                {
                    PlayerManager _player = _other.gameObject.GetComponentInParent<PlayerManager>();

                    if (_player != null)
                    {
                        ShieldController shield = GetComponentInParent<ShieldController>();
                        //i 
                        if (_player.team == Team.Virus )
                        {
                            GeneralAI _aiPlayer = _other.GetComponentInParent<GeneralAI>();
                            if(_aiPlayer != null)
                            {
                                _aiPlayer.isInsideShield = true;
                                _aiPlayer.exitVector = transform.parent.GetChild(0);
                            }
                            if(!shield.isActivated)
                                GetComponentInParent<ShieldController>().photonView.RPC("ActivateShield" , Photon.Pun.RpcTarget.AllBuffered);
                        }

                        return;
                    }
                }
            }

    }


}