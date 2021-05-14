using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace LoneX.TchilaVirus
{
    public class VirusController : MonoBehaviourPun
    {
        [SerializeField]
        public Rigidbody2D[] rigidPoint;
        public float speed;
        public float maxVelocity;
        public GameObject virusTrail;
        public SoftBody softBody;
        private Vector2 movement = new Vector2();
        public bool isFast = false;
        public bool isAI = false;
        public bool canMove = true;
        public bool isDouble = false;
        private GameObject virusDouble;
        public GameObject virusDoublePrefab;

        private void Awake()
        {
            if (photonView.IsMine && ! isAI)
                CineCamera.instance.Follow(this.transform);
        }

        private void Update()
        {
            if(!isAI)
            {   
                movement.x = SimpleInput.GetAxis("Horizontal") * Time.fixedDeltaTime * speed;
                movement.y = SimpleInput.GetAxis("Vertical") * Time.fixedDeltaTime * speed; 
            }
        }

        private void FixedUpdate()
        {
            PlayerManager _playerManager = GetComponent<PlayerManager>();
            
            if(virusDouble != null && _playerManager.isDuplicated)
            {
                virusDouble.GetComponent<VirusDoubleController>().targetMovement = this.rigidPoint[0].velocity;   
            }

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
                return;

            if(!GameManager.isGameover)
                Move();
        }

        public void Ghostify(bool _isGhost)
        {
            foreach (Rigidbody2D point in rigidPoint)
                point.simulated = !_isGhost;
        }

        private void Move()
        {
            PlayerManager _player = GetComponent<PlayerManager>();

            if (_player.isDead)
            {
                this.transform.position = this.transform.position + new Vector3(movement.x , movement.y , 0f).normalized * 0.25f;
                return;
            }

            float _xAxis = SimpleInput.GetAxisRaw("Horizontal");
            float _yAxis = SimpleInput.GetAxisRaw("Vertical");
            Vector2 _movementInput = new Vector2(_xAxis , _yAxis);
            
            if(_movementInput.magnitude == 0f && !isAI)
            {       
                return;
            }

            for (int i = 0; i < rigidPoint.Length-1; i++)
            {
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

        public void MoveAI(Vector2 _direction)
        {
            movement = _direction * speed * Time.deltaTime;
        }

        public void GenerateTrail()
        {
            GameObject vt = Instantiate(virusTrail , this.transform.position , Quaternion.identity);
            vt.GetComponent<VirusTrail>().SetPointsPositions(softBody.points);
        }

        public void CreateVirusDouble()
        {
            virusDouble = Instantiate(virusDoublePrefab , this.transform.position , Quaternion.identity);
            
            VirusDoubleController _virusDoubleController = virusDouble.GetComponent<VirusDoubleController>();
            _virusDoubleController.usernameText.text = GetComponent<PlayerManager>().playerName.text;
            _virusDoubleController.doubleMovementNormal = Vector2.Perpendicular(rigidPoint[0].velocity.normalized);
        }
    }
}
