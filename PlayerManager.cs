using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using TMPro;

namespace LoneX.TchilaVirus
{
    public class PlayerManager : MonoBehaviourPun
    {
        private float health;
        public float maxHealth;
        public Team team;
        public bool isDead;
        public bool isInvincible;
        public bool IsInvincible{get{return isInvincible;} set{animator.SetShield( value); isInvincible = value;}}
        public AudioSource audioSource;
        public AudioClip[] bounce;
        public VirusAnimator animator;
        public List<Pickable> abilities;
        public TMP_Text playerName;
        public Button[] abilityButtons = new Button[2];
        public VirusController virusController;
        public bool isFast = false;
        public int maxFastTime = 5;
        public float speedFactor;
        public GameObject virusDouble;
        public bool isDuplicated = false;
        public GameObject radar;
        public GameObject magnet;
        public GameObject projectile;
        public int projectileCount;
        public float projectileRadius;
        public float projectileSpeed;
        public AudioClip projectileShotClip;
        private float nextTrailSpriteTimer = 0.25f;
        public float nextTrailSpriteMaxTimer = 0.25f;
        public float immunityTime = 6f;
        public float magnetTime = 5;
        public AudioClip magnetActivated;
        public AudioClip goFastClip;
       
        private void Awake()
        {
            isDead = false;
            MakeImmune();
            
            radar = Instantiate(radar , this.transform.position , Quaternion.identity);
            radar.SetActive(true);
            if(team == Team.Virus)
                radar.GetComponent<Radar>().targetTeam = Team.White;
            else
                radar.GetComponent<Radar>().targetTeam = Team.Virus;
            
        }

        private void Start()
        {
            health = maxHealth;
            virusController = GetComponent<VirusController>();   
             
            if(virusController.isAI)
                GetComponent<GeneralAI>().radar = radar.GetComponent<Radar>();
        }

        private void SetRadarPosition()
        {
            radar.transform.position = this.transform.position;
        }

        private void OnEnable()
        {
            if(photonView.IsMine && ! virusController.isAI)
            {
                PickableHudSlot.Clicked += OnPickableSlotClicked;
                playerName.text = PhotonNetwork.NickName;
            }
            else
            {
                playerName.text = photonView.Owner.NickName;
            }
        }

        private void OnDisable()
        {
            if(photonView.IsMine)
            {
                PickableHudSlot.Clicked -= OnPickableSlotClicked;
            }
        }

        private void Update()
        {
            SetRadarPosition();

            if(GameManager.isGameover && !GameManager.instance.PlayerWon(this) && health > 0.0f)
            {
                ChangeHealthBy(-health);
            }
            if(isFast && !isDead)
            {
                if(nextTrailSpriteTimer <= 0.0f)
                {
                    virusController.GenerateTrail();
                    nextTrailSpriteTimer = nextTrailSpriteMaxTimer;
                }
              
                nextTrailSpriteTimer -= Time.deltaTime;
            }
        }

        private void OnCollisionEnter2D(Collision2D _other)
        {
            if (_other.gameObject != null)
            {
                PlayerManager _otherPlayer = _other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (_otherPlayer != null)
                {
                    if (this.team == Team.Virus && _otherPlayer.team == Team.White)
                    {
                        return;
                    }
                }
                int rand = UnityEngine.Random.Range(0,3);
                if(!audioSource.isPlaying)
                    PlaySound(bounce[rand]);
            }
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.gameObject != null)
            {
                PlayerManager _otherPlayer = _other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (_otherPlayer != null && !_other.usedByEffector)
                {
                    //if player is on the other team and you're a virus
                    if (this.team == Team.Virus && _otherPlayer.team == Team.White)
                    {
                        ChangeHealthBy(-10f);
                        //Debug.Log($"Health == {health}");
                    }

                    return;
                }
            }
        }
        
        public void ChangeHealthBy(float _value)
        {
            if(_value < 0f && isInvincible)
                return;
                
            if (photonView.IsMine )
                photonView.RPC("SyncHealth", RpcTarget.AllBuffered, health + _value);
        }

        [PunRPC]
        public void SyncHealth(float _health)
        {
            this.health = Mathf.Clamp(_health, 0f, maxHealth);
            if (health == 0f)
                Die();
        }
        
        private void PlaySound(AudioClip _clip)
        {
                audioSource.PlayOneShot(_clip);
        }
        
        private void Die()
        {
            isFast = false;
            this.GetComponent<PhotonTransformView>().enabled = true;
            this.GetComponent<PhotonRigidbody2DView>().enabled = false;
            isDead = true;
            animator.Die(false);
            radar.gameObject.SetActive(false);
            //Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} died y'all");
        }

        public void AddAbility (Pickable _pickable)
        {
            if(abilities.Count < 2 && photonView.IsMine)
            {
                abilities.Add(_pickable);
                //Debug.Log($"your player has picked a pcikable of type {_pickable.pickableType}");
                HUD.instance.UpdatePickedItems(abilities);
            }
        }

         public void OnPickableSlotClicked( object _source , EventArgs args )
         {
            PickableHudSlot _sourceSlot = (PickableHudSlot) _source;
            Pickable _ability = _sourceSlot.item;

            UseAbbility(_ability);
            _sourceSlot.EmptySlot();
         }

         public void UseAbbility(Pickable _ability)
         {
            switch(_ability.pickableType)
            {
                case PickableType.Magnesium :
                    photonView.RPC("ActivateMagnet", RpcTarget.All);
                    //Debug.Log("used magnesium!");
                    break;
                case PickableType.Sulfur : 
                    photonView.RPC("ShootMultidirectional", RpcTarget.All);
                    //Debug.Log("used Sulfur");
                    break;
                case PickableType.Glucose : 
                    photonView.RPC("GoFast", RpcTarget.All);
                    break;
                case PickableType.Proteine :
                    photonView.RPC("Duplicate", RpcTarget.All);
                    break;
                case PickableType.Vitamin :
                    if(team == Team.Virus)
                    {
                        photonView.RPC("MakeImmune" , RpcTarget.All);
                    }
                    break;
                default :
                    //Debug.Log("Uknown ability");
                    break;
            }

             abilities.Remove(_ability);
         }
         
         [PunRPC]
         public void GoFast()
         {
            if(!isFast)
                virusController.speed *= speedFactor;
            
            isFast = true;
            virusController.isFast = true;
            animator.GlowOnce();
            PlaySound(goFastClip);
            nextTrailSpriteTimer = nextTrailSpriteMaxTimer;
            StartCoroutine(StopTrailEffectIn(maxFastTime));
         }
         
         [PunRPC]
         public void MakeImmune()
         {
            IsInvincible = true;
            animator.GlowOnce();
            StartCoroutine(MakeImmuneEndIn(immunityTime));
         }

         [PunRPC]
         public void ShootMultidirectional()
         {
            float _angleStep = 360f / projectileCount;
            float _angle = 0f;

            animator.Shoot();
            audioSource.PlayOneShot(projectileShotClip);

            for (int i = 0; i <= projectileCount; i++)
            {
                float _projectileDirX = transform.position.x + Mathf.Sin((_angle * Mathf.PI) / 180 ) * projectileRadius;
                float _projectileDirY = transform.position.y + Mathf.Cos((_angle * Mathf.PI) / 180 ) * projectileRadius;
                
                Vector2 _currentPosition = new Vector2(transform.position.x,transform.position.y);
                Vector2 _projectileVector = new Vector2(_projectileDirX , _projectileDirY);
                Vector2 _projectileMoveDir = (_projectileVector - _currentPosition).normalized * projectileSpeed;
                
                var _projectile = Instantiate(projectile, _projectileVector , Quaternion.identity);                
                _projectile.GetComponent<Rigidbody2D>().velocity = _projectileMoveDir;

                _angle += _angleStep;
            }
         }

         [PunRPC]        
         public void ActivateMagnet()
         {
             if(this.team == Team.White)
             {
                magnet.SetActive(true);
                PlaySound(magnetActivated);
                animator.Glow();
                StartCoroutine(DeactivateMagnet(magnetTime));
             }
         }

         [PunRPC]
         public void Duplicate()
         {
             if(team == Team.Virus)
             {
                isDuplicated = true;
                animator.GlowOnce();
                virusController.CreateVirusDouble();
             }
         }
         
         public IEnumerator MakeImmuneEndIn(float _delay)
         {
            yield return new WaitForSeconds(_delay);
            IsInvincible = false;
         }
         public IEnumerator DeactivateMagnet(float _delay)
         {
             if(this.team == Team.White)
             {
                 yield return new WaitForSeconds(_delay);
                 animator.StopGlow();
                 yield return new WaitForSeconds(0.2f);
                 magnet.SetActive(false);
             }
         }
         public IEnumerator StopTrailEffectIn(int _delay)
         {
             yield return new WaitForSeconds(_delay);
             isFast = false;
             virusController.isFast = false;
             virusController.speed *= 1 / speedFactor;
         }
    }

}