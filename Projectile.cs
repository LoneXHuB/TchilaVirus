using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoneX.TchilaVirus
{
    public class Projectile : MonoBehaviour
    {
        public ParticleSystem exploder;
        public Rigidbody2D rigid;
        public SpriteRenderer spriteRenderer;
        public Team targetTeam;
        public void Awake()
        {
            Destroy(this.gameObject , 3f);
        }

        public void OnTriggerEnter2D(Collider2D _other)
        {
            PlayerManager _otherPlayer = _other.gameObject.GetComponentInParent<PlayerManager>();

            if(_otherPlayer != null)
            {
                if(_otherPlayer.team == targetTeam)
                {
                    _otherPlayer.ChangeHealthBy( - 10f); 
                    Debug.Log($"projectile hit player of team {_otherPlayer.team}");
                }
            }
            
            StartCoroutine(Kill(3f));
        }

        public IEnumerator Kill(float _delay)
        {
            Destroy(rigid);
            Destroy(spriteRenderer);
            Destroy(this.GetComponent<Collider2D>());
            exploder.Emit(30);
            
            yield return new WaitForSeconds(_delay);
            Destroy(this.gameObject);
        }
    }

}
