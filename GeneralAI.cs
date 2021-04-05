using System;
using UnityEngine;
using Pathfinding;
using Photon.Pun;

namespace LoneX.TchilaVirus
{
    public enum AIState  { OnDuty , Threatened , Curious , Chasing}
    public class GeneralAI : MonoBehaviourPun
    {
        public Transform target;
        public Transform secondaryTarget;
        public float nextWayPointDistance = 3f;
        public static Int32[] activatedShields;
        public Radar radar;
        public Team team;
        public float fleeDistance = 15f;
        public Transform fleeTarget;
        public float minAttackDistance = 10f;
        public PhotonView photonView;

        Transform enemyPosition;
        Transform foundPickable;
        Transform closeShieldPosition;
        Vector3 fleeVector;
        Path path;
        int currentWayPoint = 0;
        bool reachedEndOfPath = false;
        AIState stateMachine = AIState.OnDuty;
        Seeker seeker;
        VirusController controller;
        PlayerManager aiPlayerManager;
        
        private void Start()
        {
            seeker = GetComponent<Seeker>();
            controller = GetComponent<VirusController>();
            
            activatedShields = new Int32[ShieldsManager.instance.transform.childCount];
            aiPlayerManager =GetComponent<PlayerManager>();
            team = aiPlayerManager.team;
            if(team == Team.White)
                minAttackDistance *= 2f;
                
            if(PhotonNetwork.IsMasterClient);
                InvokeRepeating("UpdatePath", 0f , .5f);
        }

        private void FindRedShieldTarget()
        {
            if(target != null)
            {
                if(team == Team.White)
                {
                    int[] _currentTarget = {target.GetSiblingIndex()};
                    int _randomShieldIndex = GetRandomExcluding(_currentTarget);
                    target = ShieldsManager.instance.transform.GetChild(_randomShieldIndex);
                    return;
                }

                bool _isTargetActivated = target.gameObject.GetComponent<ShieldController>().isActivated;
                
                if(!_isTargetActivated)
                    return;
            }

            int _randomShieldNumber = GetRandomExcluding(activatedShields);
            target = ShieldsManager.instance.transform.GetChild(_randomShieldNumber);
        }

        private int GetRandomExcluding(Int32[] _exclusions)
        {
            
            bool _found = false;
            int _randInt = UnityEngine.Random.Range(0 , ShieldsManager.instance.transform.childCount);
            if(team == Team.Virus)
            {
                while(!_found)
                {
                    _found = true;

                    foreach(Int32 _excluded in activatedShields)
                    {
                        if(_excluded == _randInt)
                        {
                            _found = false;
                            break;
                        }
                    }
                }
            }

            return _randInt;
        }

        private void UpdatePath()
        {
            ExecuteStateBehaviour();
            
            if(seeker.IsDone() && ! aiPlayerManager.isDead)
            {
                if(stateMachine == AIState.OnDuty)
                    seeker.StartPath(this.transform.position , target.position , OnPathComplete);
                else
                    seeker.StartPath(this.transform.position , secondaryTarget.position , OnPathComplete);
            }
        }

        private void OnPathComplete(Path _path)
        {
            if(!_path.error)
            {
                path = _path;
                currentWayPoint = 0;
            }
            else
                Debug.Log("Error calculating path");
        }

        private void FixedUpdate()
        {
            if(!PhotonNetwork.IsMasterClient)
                return;
            if(aiPlayerManager.isDead)
                {
                    controller.MoveAI(Vector2.zero);
                    return;
                }
            AnalyseEnvironement();
            WalkPath();
        }

        public void AnalyseEnvironement()
        {
            fleeVector = Vector2.zero;
            if(radar == null)
                return;

            foreach(Collider2D _detectedEntity in radar.detectedEntities)
            {
                if(_detectedEntity == null)
                    continue;
                
                PlayerManager _player = _detectedEntity.GetComponentInParent<PlayerManager>();
                Pickable _pickable = _detectedEntity.GetComponent<Pickable>();
                ShieldController _shield = _detectedEntity.GetComponent<ShieldController>();

                if(_pickable != null)
                {
                    bool _takePickable = true;
                    foundPickable = _pickable.transform;
                    if(stateMachine != AIState.Threatened && aiPlayerManager.abilities.Count < 2)
                        {
                            if(team == Team.Virus)
                            {
                                if(_pickable.pickableType == PickableType.Magnesium)
                                    _takePickable = false;
                            }
                            else if(team == Team.White)
                            {
                                if(_pickable.pickableType == PickableType.Vitamin || _pickable.pickableType == PickableType.Proteine)
                                    _takePickable = false;
                            }

                            if(_takePickable)
                                stateMachine =AIState.Curious;
                        }
                }
                if(_player != null)
                {
                    if(this.team != _player.team && this.team == Team.Virus)
                    {
                        fleeVector+= this.transform.position - _player.transform.position;
                        stateMachine = AIState.Threatened;
                        
                        if(Vector2.Distance(this.transform.position , _player.transform.position) <= minAttackDistance)
                            {
                                Debug.Log("EnemyToo Close // PANICK! ");
                
                                try
                                {
                                    Pickable _ability0 = aiPlayerManager.abilities[0];
                                    Pickable _ability1 = aiPlayerManager.abilities[1];
                                    aiPlayerManager.UseAbbility(_ability0);
                                    aiPlayerManager.UseAbbility(_ability1);
                                }catch{}
                            }
                    }
                    else if(this.team != _player.team && this.team == Team.White)
                    {
                        enemyPosition = _player.transform;
                        stateMachine = AIState.Chasing;
                    }
                }
                if(_shield != null)
                {
                    closeShieldPosition = _shield.transform;
                    if(this.team == Team.White && (target == closeShieldPosition))
                       {
                            FindRedShieldTarget();
                            Debug.Log("too close to shield changing target !");
                       }
                }
            }

            fleeTarget.position = this.transform.position + fleeVector.normalized * fleeDistance; 
        }
        private void StateDependentPathCorrection()
        {
            switch (stateMachine)
            {
                case AIState.OnDuty :
                    int _lastWayPoint = path.vectorPath.Count - 1;
                    path.vectorPath[_lastWayPoint - 1] = target.GetChild(0).position;
                    
                    Vector2 _entryHelper0 = target.GetChild(1).position;
                    Vector2 _entryHelper1 = target.GetChild(2).position;

                    float _helper0Dist = Vector2.Distance(this.transform.position , _entryHelper0);
                    float _helper1Dist = Vector2.Distance(this.transform.position , _entryHelper1);
                    
                    if(currentWayPoint < _lastWayPoint -2 )
                    {
                        if(Mathf.Min(_helper0Dist,_helper1Dist) == _helper0Dist)
                            path.vectorPath[_lastWayPoint - 2] = _entryHelper0;
                        else
                            path.vectorPath[_lastWayPoint - 2] = _entryHelper1;
                    }

                    break;
                case AIState.Threatened :
                    _lastWayPoint = path.vectorPath.Count - 1;
                    Vector2 _lastWayPointPosition = path.vectorPath[_lastWayPoint];
                    
                    RaycastHit2D _hit = Physics2D.Raycast(path.vectorPath[_lastWayPoint - 1] , _lastWayPointPosition);
                    fleeVector += new Vector3(_hit.normal.x , _hit.normal.y , 0f);
                    
                    break;

                default : break;
            }
        }

        private void ExecuteStateBehaviour()
        {
            switch (stateMachine)
            {
                case AIState.OnDuty :
                    if(team == Team.Virus || target == null)
                    FindRedShieldTarget();
                    break;

                case AIState.Threatened :
                    if(team == Team.Virus)
                        {
                            fleeTarget.position = this.transform.position + fleeVector.normalized * fleeDistance; 
                            secondaryTarget =  fleeTarget;
                        }
                    break;

                case AIState.Curious : 
                    if(aiPlayerManager.abilities.Count < 2)
                        secondaryTarget = foundPickable;
                    break;

                case AIState.Chasing : 
                    secondaryTarget = enemyPosition;
                    break;

                default : break;
            }
        }

        private void WalkPath()
        {
            if(path == null)
                return;

            if(currentWayPoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                stateMachine = AIState.OnDuty;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }
            
            StateDependentPathCorrection();

            Vector2 _direction = ((Vector3)path.vectorPath[currentWayPoint] - this.transform.position).normalized;
            controller.MoveAI(_direction);

            float _distance = Vector2.Distance(this.transform.position , path.vectorPath[currentWayPoint]);

            if(_distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }   
            
        }

        public void OnThreatenedBehaviour()
        {

        }
    }
}