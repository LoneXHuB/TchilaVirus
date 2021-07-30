using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus
{
    public class VirusAnimator : MonoBehaviour
    {
        public GameObject spriteShape;
        public GameObject internals;

        public Animator animator;
        public AudioSource audioSource;
        public AudioClip deathClip;

        public void Die(bool isDouble)
        {
            spriteShape.SetActive(false);
            internals.SetActive(false);
            animator.SetTrigger("Die");
            PlaySound(deathClip);
            
            if(!isDouble)
            {
                VirusController controller = GetComponentInParent<VirusController>();
                controller.Ghostify(true);    
            }
        }

        public void Shoot()
        {
           animator.SetTrigger("GlowOnce");
        }
        public void Glow()
        {
            animator.SetTrigger("Glow");
        }
        public void StopGlow()
        {
            animator.SetTrigger("StopGlow");
        }
        public void GlowOnce()
        {
            animator.SetTrigger("GlowOnce");
        }
        
        public void SetShield(bool _isImmune)
        {
            animator.SetBool("Immune" , _isImmune);
        }

        public void PlaySound(AudioClip _clip)
        {
            audioSource.PlayOneShot(_clip);
        }

    }
}
