﻿using System;
using UnityEngine;
using Pathfinding;
using Photon.Pun;

namespace LoneX.TchilaVirus
{
    public enum AIState  { OnDuty , Threatened , Curious , Chasing , SeekingRefuge}
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
        public bool isInsideShield = false;
        public Transform exitVector;
        Transform refugeShield;
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
                
            if(PhotonNetwork.IsMasterClient)
                InvokeRepeating("UpdatePath", 0f , .5f);
        }

        private void FindRedShieldTarget()
        {
            if(!GameManager.isGameover)
            {
                int _randomShieldNumber = 0;
                if(target != null)
                {
                    ShieldController _targetShield = target.gameObject.GetComponent<ShieldController>();
                       if(!_targetShield.isActivated && team == Team.Virus)
                            return;
                            
                    _randomShieldNumber =  GetRandomExcept(target.GetSiblingIndex());
                }
                else _randomShieldNumber = GetRandomExcept(-1);
                
                target = ShieldsManager.instance.transform.GetChild(_randomShieldNumber);
            }
        }

        private int GetRandomExcept(int _exception)
        {
            int _randomShieldNumber =  UnityEngine.Random.Range(0 , ShieldsManager.instance.transform.childCount);
            if(team == Team.White)
            {
                if(_randomShieldNumber == _exception)
                    return GetRandomExcept(_exception);
                else
                    return _randomShieldNumber;
            }
            else return _randomShieldNumber;
        }

        private void UpdatePath()
        {
            ExecuteStateBehaviour();
            
            if(seeker.IsDone() && ! aiPlayerManager.isDead)
            {
                if(stateMachine == AIState.OnDuty)
                   {
                       ShieldController _targetShield = target.gameObject.GetComponent<ShieldController>();
                       if(_targetShield.isActivated && team == Team.Virus)
                            FindRedShieldTarget();
                        seeker.StartPath(this.transform.position , target.position , OnPathComplete);
                   }
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
            if(aiPlayerManager.isDead || GameManager.isGameover)
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
            stateMachine = AIState.OnDuty;
            if(radar == null)
                return;

            int _index = 0;

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
                        //Debug.Log("too close to shield changing target !");
                    }
                    else if(this.team == Team.Virus && stateMachine == AIState.Threatened)
                    {
                        if(stateMachine == AIState.Threatened)
                        {
                            stateMachine = AIState.SeekingRefuge;
                            refugeShield = closeShieldPosition;
                            //Debug.Log("Virus is seeking refuge");
                        }
                        else stateMachine = AIState.OnDuty;

                    }
                }

                radar.detectedEntities[_index] = null;
                _index++;
            }

            fleeTarget.position = this.transform.position + fleeVector.normalized * fleeDistance; 
        }

        private void StateDependentPathCorrection()
        {
            switch (stateMachine)
            {
                case AIState.OnDuty :
                    
                    if(reachedEndOfPath)
                        break;

                    int _lastWayPoint = path.vectorPath.Count - 1;
                    try
                    {
                        path.vectorPath[_lastWayPoint - 1] = target.GetChild(0).position;
                    }
                    catch { return; }
                    
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
                     if(isInsideShield)
                    {
                        Vector2 _exitHelper0 = exitVector.position;
                        Vector2 _exitHelper1 = exitVector.position;

                        _helper0Dist = Vector2.Distance(this.transform.position , _exitHelper0);
                        _helper1Dist = Vector2.Distance(this.transform.position , _exitHelper1);
                        if(currentWayPoint < 2)
                        {
                            path.vectorPath[1] = exitVector.position;
                            if(Mathf.Min(_helper0Dist,_helper1Dist) == _helper0Dist)
                                path.vectorPath[2] = _exitHelper0;
                            else
                                path.vectorPath[2] = _exitHelper1;
                        }
                        if(Vector3.Distance(this.transform.position , exitVector.position) < nextWayPointDistance)
                            isInsideShield =false;
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
            //Debug.Log($"team : {team} \n stateMachine : {stateMachine}");

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
                case AIState.SeekingRefuge :
                    secondaryTarget = closeShieldPosition;
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
    }
}