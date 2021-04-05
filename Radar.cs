using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus
{
    public class Radar : MonoBehaviour
    {
        public WarningSign warning;
        public Team targetTeam;
        public Collider2D[] detectedEntities;
        public Collider2D collider;

        
         void Awake()
        {
            warning.gameObject.SetActive(false);
        }

        public void OnTriggerEnter2D(Collider2D _other)
        {
            PlayerManager _detectedPlayer = _other.GetComponentInParent<PlayerManager>();
         
            if( _detectedPlayer != null )
            {
                if(_detectedPlayer.team == targetTeam)
                {   
                    Debug.Log("Radar Trigger entered! ");
                    float _distance = (this.transform.position - _detectedPlayer.transform.position).magnitude;
                    warning.gameObject.SetActive(true);
                }
            }
        }

        public void OnTriggerStay2D(Collider2D _other)
        {
            detectedEntities = new Collider2D[10];
            Physics2D.GetContacts(this.collider , detectedEntities);
            PlayerManager _detectedPlayer = _other.GetComponentInParent<PlayerManager>();

            if( _detectedPlayer != null )
            {
                if(_detectedPlayer.team == targetTeam)
                {
                    float _distance = (this.transform.position - _detectedPlayer.transform.position).magnitude;
                    WarnPlayer(_distance);
                }
            }
        }

        public void OnTriggerExit2D(Collider2D _other)
        {
            PlayerManager _detectedPlayer = _other.GetComponentInParent<PlayerManager>();

            if( _detectedPlayer != null )
            {
                if(_detectedPlayer.team == targetTeam)
                {
                    float _distance = (this.transform.position - _detectedPlayer.transform.position).magnitude;
                    warning.gameObject.SetActive(false);
                }
            }
        }

        public void WarnPlayer(float _distance)
        {
            warning.gameObject.SetActive(true);
            
            float _r = GetComponent<CircleCollider2D>().radius;

            if(_distance < _r)
            {
               float _animationSpeed = 1 / ((0.9f/ _r) * _distance + 0.1f);

                warning.SetAnimationSpeed(_animationSpeed);
            }
            else
            {
                warning.SetAnimationSpeed(-1f);    
            }
        }   
    }    
}