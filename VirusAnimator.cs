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

        public Animator deathAnimator;
        public AudioSource audioSource;
        public AudioClip deathClip;

        public void Die(bool isDouble)
        {
            spriteShape.SetActive(false);
            internals.SetActive(false);
            deathAnimator.SetTrigger("Die");
            PlaySound(deathClip);
            
            if(!isDouble)
            {
                VirusController controller = GetComponentInParent<VirusController>();
                controller.Ghostify(true);    
            }
        }

        public void Shoot()
        {
            //TODO : implement shoot animation
        }

        public void PlaySound(AudioClip _clip)
        {
            audioSource.PlayOneShot(_clip);
        }

    }
}
