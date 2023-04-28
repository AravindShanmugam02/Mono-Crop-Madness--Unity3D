using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

[System.Serializable]
public class CorpIndusIdleState : CorpIndusAbstractBaseState
{
    private GameTimer _gameTimer;
    private GameTimer _ownTimerForFarmStateTransition;
    private GameTimer _ownTimerForPatrolStateTransition;

    [SerializeField] private float _farmTimer;
    [SerializeField] private float _patrolTimer;

    private NavMeshAgent _navMeshAgent;
    [SerializeField] private bool _isFirstTimeFarming = true;
    [SerializeField] private bool _isFirstTimePatroling = true;

    public override void EnterState(CorpIndusStateManager corpIndusStateManager)
    {
        //_gameTimer = corpIndusStateManager.GetGameTimer();
        //if(_gameTimer != null && _ownTimerForFarmStateTransition == null) { _ownTimerForFarmStateTransition = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer); }
        //if(_gameTimer != null && _ownTimerForPatrolStateTransition == null) { _ownTimerForPatrolStateTransition = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer); }

        _navMeshAgent = corpIndusStateManager.GetNavMeshAgent();
        _navMeshAgent.SetDestination(corpIndusStateManager.GetIdleStateStartingPosition().position);
    }

    public override void CollisionEnter(CorpIndusStateManager corpIndusStateManager, Collision collision)
    {

    }

    public override void UpdateState(CorpIndusStateManager corpIndusStateManager)
    {
        if (CheckWeatherNavMeshAgentHasReachedDestination())
        {
            if (_gameTimer != null && _ownTimerForFarmStateTransition != null && _ownTimerForPatrolStateTransition != null)
            {
                //AI can't go for Patrol & Farming post end of the day!
                //At the end of the day patrol timer and the farm timer will be reset to 0;
                if (!corpIndusStateManager.ShouldWeCallForTheDay())
                {
                    _farmTimer = TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownTimerForFarmStateTransition, _gameTimer);
                    _patrolTimer = TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownTimerForPatrolStateTransition, _gameTimer);

                    //Testing purpose...
                    //_farmTimer = 0;
                    //_patrolTimer = 0;

                    if (_isFirstTimeFarming == true || _isFirstTimePatroling == true) //When use is playing for the first time.
                    {
                        if (_farmTimer > 120f) //2nd hour - Farm
                        {
                            _ownTimerForFarmStateTransition = null;
                            _farmTimer = 0f;
                            _isFirstTimeFarming = false;
                            corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIFarmState);
                        }
                        else if (_patrolTimer > 240f) //4th hour - Patrol
                        {
                            _ownTimerForPatrolStateTransition = null;
                            _patrolTimer = 0f;
                            _isFirstTimePatroling = false;
                            corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIPatrolState);
                        }
                    }
                    else if (_farmTimer > 180f && _patrolTimer <= 300f) //3 hours - Farm Routine
                    {
                        _ownTimerForFarmStateTransition = null;
                        _farmTimer = 0f;
                        corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIFarmState);
                    }
                    else if (_patrolTimer > 300f && _farmTimer <= 180f) //5 hours - Patrol Routine
                    {
                        _ownTimerForPatrolStateTransition = null;
                        _patrolTimer = 0f;
                        corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIPatrolState);
                    }
                    else if (_farmTimer > 180f && _patrolTimer > 300f) //If time comes for both farm and patrol, AI will choose farm as the state to switch
                    {
                        _ownTimerForFarmStateTransition = null;
                        _farmTimer = 0f;
                        corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIFarmState);
                    }
                }
                else
                {
                    //Its Night / MidNight Now. Hence no farming!
                    //Debug.Log("Its Night / MidNight Now. Hence no new farming state for Crop Industry!");

                    //At the end of the day patrol timer and the farm timer will be reset to 0...
                    _farmTimer = 0f;
                    _patrolTimer = 0f;

                    //Game timer instances for farm and patrol won't be set to null, as the next morning AI will go to do farm activities in the land...
                    //Hence not setting _ownTimerForPatrolStateTransition & _ownTimerForFarmStateTransition to null.
                }
            }
            else
            {
                if (_gameTimer == null) { _gameTimer = corpIndusStateManager.GetGameTimer(); }
                if (_gameTimer != null && _ownTimerForFarmStateTransition == null) { _ownTimerForFarmStateTransition = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer); }
                if (_gameTimer != null && _ownTimerForPatrolStateTransition == null) { _ownTimerForPatrolStateTransition = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer); }
            }
        }
    }

    bool CheckWeatherNavMeshAgentHasReachedDestination()
    {
        //http://answers.unity.com/answers/746157/view.html
        //if no path pending
        if (!_navMeshAgent.pathPending)
        {
            //if remaining distance is less than or equal to stoping distance (i.e (0 <= 0))
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                //if agent has no path or if agent's velocity in magnitude is 0 (i.e not moving)
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}