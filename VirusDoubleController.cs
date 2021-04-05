using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace LoneX.TchilaVirus
{

    public class VirusDoubleController : MonoBehaviour
    {
        [SerializeField]
        public Rigidbody2D[] rigidPoint;
        public float speed;
        public float maxVelocity;
        public SoftBody softBody;
        public Vector2 movement = new Vector2();
        public bool isFast = false;
        public Vector2 doubleMovementNormal;
        public Vector2 targetMovement;
        public VirusAnimator animator;
        public SpriteRenderer eyes;
        public Canvas canvas;
        public TMP_Text usernameText;

        public void Awake()
        {
            StartCoroutine(FadeInSeconds(4f));
        }
        void FixedUpdate()
        {
            Vector3 _dmn = new Vector3 (doubleMovementNormal.x,doubleMovementNormal.y,0);
            Vector3 _mov = new Vector3 (movement.x , movement.y , 0f);
            Vector3 _tar = new Vector3 (targetMovement.x , targetMovement.y , 0f);

           /* Debug.DrawLine(this.transform.position , this.transform.position + _dmn, Color.red, 0f, false);
            Debug.DrawLine(this.transform.position , this.transform.position + _mov, Color.blue, 0f, false);
            Debug.DrawLine(this.transform.position , this.transform.position + _tar, Color.green, 0f, false);
           */
            movement = targetMovement - 2 * (Vector2.Dot(targetMovement , doubleMovementNormal)) * doubleMovementNormal.normalized;
          
            Move();
        }

        private void Move()
        {
            for (int i = 0; i < rigidPoint.Length-1; i++)
            {
                //TODO change to a test to see if the joystick is being activated
                if(movement.magnitude < 8f )
                {       
                    return;
                }
                if(Vector2.Dot(rigidPoint[i].velocity , movement) != 1 && Vector2.Dot(rigidPoint[i].velocity , movement) != 0f )
                {
                    rigidPoint[i].velocity = Vector2.zero;
                }

                if (rigidPoint[i].velocity.magnitude <= maxVelocity || (isFast && rigidPoint[i].velocity.magnitude <= maxVelocity * 2f))
                {
                    rigidPoint[i].AddForce(movement, ForceMode2D.Impulse);
                }
            }
        }

        public IEnumerator FadeInSeconds(float _delay)
        {
            yield return new WaitForSeconds(_delay);
            animator.Die(true);
            Destroy(eyes.gameObject);
            Destroy(canvas.gameObject);
            Destroy(this.gameObject , 1f);
        }

    }

}