using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus

{
    public class WarningSign : MonoBehaviour
    {
        public Animator animator;


        public void SetAnimationSpeed(float _speed)
        {
            animator.SetFloat("Frequency" , _speed);
        }

    }   
}
