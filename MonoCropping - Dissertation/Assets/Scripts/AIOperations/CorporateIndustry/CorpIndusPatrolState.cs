using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

[System.Serializable]
public class CorpIndusPatrolState : CorpIndusAbstractBaseState
{
    public PlayerController GetPlayerController() { return _interactor.GetHitObjectInfoFromCorpIndusInteractor().collider.GetComponent<PlayerController>(); }   
    public void SetInteractionComplete(bool toggle) { _isInteractionComplete = toggle; }
    public void SetInteracting(bool toggle) { _isInteracting = toggle; }


    private NavMeshAgent _navMeshAgent;
    [SerializeField] private List<Transform> _waypointsList;
    private CorpIndusInteractor _interactor;

    [SerializeField] private bool _isInteractionComplete = false;
    [SerializeField] private bool _isInteracting = false;
    [SerializeField] private bool _ranOnce = false;

    //Sub State handle to hold current sub state information...
    private CorpIndusAbstractPatrolSubState _currentCISubState;

    //Handle for sub states...
    private CorpIndusPatrolStateInteractSubState _cIPatrolSubStateInteractState;

    //Handle for IEnumerator...
    IEnumerator e;

    public override void EnterState(CorpIndusStateManager corpIndusStateManager)
    {
        if(_navMeshAgent == null)
        {
            _navMeshAgent = corpIndusStateManager.GetNavMeshAgent();
            _navMeshAgent.SetDestination(corpIndusStateManager.GetPatrolStateStartingPosition().position);
        }
        
        if(_waypointsList == null || _waypointsList.Count == 0)
        {
            _waypointsList = new List<Transform>();
            _waypointsList = corpIndusStateManager.GetPatrolStateWaypoints();
        }

        if(_interactor == null)
        {
            _interactor = corpIndusStateManager.GetInteractorOfCorpIndus().GetComponent<CorpIndusInteractor>();
        }

        if(_cIPatrolSubStateInteractState == null)
        {
            _cIPatrolSubStateInteractState = new CorpIndusPatrolStateInteractSubState();
        }

        if (e == null)
        {
            e = StartPatrol(corpIndusStateManager);
        }

        _isInteractionComplete = false;
        _isInteracting = false;
        _ranOnce = false;

        //https://www.youtube.com/watch?v=vQU9DjX8xYI&ab_channel=SteelResolveGames
        //Starting coroutine this way helps to stop and resume the coroutine from its previous execusion point.
        corpIndusStateManager.StartCoroutine(e);
    }

    public override void CollisionEnter(CorpIndusStateManager corpIndusStateManager, Collision collision)
    {

    }

    public override void UpdateState(CorpIndusStateManager corpIndusStateManager)
    {
        if (CheckIfInteracting() && !CheckInteractionCompleted() && e != null && !CheckIfPlayerIsAlreadyWarned())
        {
            if (!_ranOnce)
            {
                _ranOnce = true;
                corpIndusStateManager.StopCoroutine(e);
                SwitchSubStates(_cIPatrolSubStateInteractState, corpIndusStateManager, this);
            }
        }
        else if(!CheckIfInteracting() && CheckInteractionCompleted() && e != null)
        {
            _currentCISubState = null;
            _isInteracting = false;
            _isInteractionComplete = false;
            _ranOnce = false;

            corpIndusStateManager.StartCoroutine(e);
        }

        UpdateSubState(corpIndusStateManager, this);
    }

    void SwitchSubStates(CorpIndusAbstractPatrolSubState state, CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState)
    {
        _currentCISubState = state;
        _currentCISubState.EnterSubState(corpIndusStateManager, corpIndusPatrolState);
    }

    void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState)
    {
        if(_currentCISubState != null)
        {
            _currentCISubState.UpdateSubState(corpIndusStateManager, corpIndusPatrolState);
        }
    }

    IEnumerator StartPatrol(CorpIndusStateManager corpIndusStateManager)
    {
        yield return new WaitUntil(CheckWeatherNavMeshAgentHasReachedDestination);

        foreach (var item in _waypointsList)
        {
            _navMeshAgent.SetDestination(item.position);

            yield return new WaitUntil(CheckWeatherNavMeshAgentHasReachedDestination);

            yield return new WaitForSeconds(5f);
        }

        e = null;
        _currentCISubState = null;

        _isInteractionComplete = false;
        _isInteracting = false;

        corpIndusStateManager.SwitchStateTo(corpIndusStateManager._cIIdleState);
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

    bool CheckIfInteracting()
    {
        if (_interactor.GetIsPlayerHit())
        {
            _isInteracting = true;
            _isInteractionComplete = false;
        }
        else
        {
            _isInteracting = false;

            //_isInteractionComplete will be set to true by the interactor sub state because that way the object will stop coroutine until the interaction action is complete.
        }
        return _isInteracting;
    }

    bool CheckInteractionCompleted()
    {
        return _isInteractionComplete;
    }

    bool CheckIfPlayerIsAlreadyWarned()
    {
        return _interactor.GetPlayerController().GetWarnedStatusOfPlayer();
    }
}