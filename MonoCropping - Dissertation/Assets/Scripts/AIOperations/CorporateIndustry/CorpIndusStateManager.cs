using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

public class CorpIndusStateManager : MonoBehaviour, ITimer
{
    public bool GetIsStartOfNewSeason() { return _isNewSeason; }
    public void SetIsStartOfNewSeason(bool value) { _isNewSeason = value; }
    public GameTimer GetGameTimer() { return _gameTimer; }
    public void SwitchStateTo(CorpIndusAbstractBaseState state) { SwitchState(state); }
    public NavMeshAgent GetNavMeshAgent() { return _navMeshAgent; }
    public Transform GetIdleStateStartingPosition() { return _idleStateStartingPosition.transform; }
    public Transform GetFarmStateStartingPosition() { return _farmStateStartingPosition.transform; }
    public Transform GetPatrolStateStartingPosition() { return _patrolStateStartingPosition.transform; }
    public Transform GetInteractStateStartingPosition() { return _farmStateStartingPosition.transform; }
    public List<Transform> GetPatrolStateWaypoints() { return _patrolStateWaypointsList; }
    public List<LandManager> GetAILandManagerList() { return _aILandManagerList; }
    public List<PlotManager> GetAIPlotManagerList() { return _aIPlotManagerList; }
    public SeedItem GetSeedItem1() { return _seedItem1; }
    public SeedItem GetSeedItem2() { return _seedItem2; }
    public GameObject GetInteractorOfCorpIndus() { return _corpIndusInteractor; }
    public bool ShouldWeCallForTheDay() { return CheckIfTimeToCallTheDay(); }

    //Handles for different states of CorporateIndustry...
    [Header("States")]
    public CorpIndusIdleState _cIIdleState;
    public CorpIndusPatrolState _cIPatrolState;
    public CorpIndusFarmState _cIFarmState;
    public CorpIndusPatrolStateInteractSubState _cITalkInteractState;
    private CorpIndusAbstractBaseState _currentCIState; //Base State handle to hold current state information...
    [SerializeField] private string _currentState; //current state indicator in string...

    [Header("State Manager")]
    [SerializeField] private List<LandManager> _aILandManagerList; //AI Lands handle list...
    [SerializeField] private List<PlotManager> _aIPlotManagerList; //AI Plots handle list...
    [SerializeField] private SeedItem _seedItem1;
    [SerializeField] private SeedItem _seedItem2;
    private NavMeshAgent _navMeshAgent; //Handle for NavMeshAgent...
    private GameObject _aIFarmManager; //AI Farm Manager handle...
    private GameTimer _gameTimer; //Game Timer Instance handle...
    private bool _isNewSeason; //New Season handle...

    //Handle for different state's starting destination...
    private GameObject _idleStateStartingPosition;
    private GameObject _farmStateStartingPosition;
    private GameObject _patrolStateStartingPosition;
    private GameObject _interactStateStartingPosition;

    //Handle for holding waypoints positions for patrol state...
    private List<Transform> _patrolStateWaypointsList;

    //Handle for the interactor object...
    private GameObject _corpIndusInteractor;

    Light _aiLight;

    // Start is called before the first frame update
    void Start()
    {
        //Initializing different states...
        _cIIdleState = new CorpIndusIdleState();
        _cIPatrolState = new CorpIndusPatrolState();
        _cIFarmState = new CorpIndusFarmState();
        _cITalkInteractState = new CorpIndusPatrolStateInteractSubState();

        //Initializing the handle for NavMeshAgent...
        _navMeshAgent = GetComponent<NavMeshAgent>();

        //Initializing AI Farm Manager handle...
        _aIFarmManager = GameObject.Find("AIFarmManager");

        //Initializing AI Plot & Land Manager handles...
        _aIPlotManagerList = _aIFarmManager?.GetComponentsInChildren<PlotManager>().ToList();
        _aILandManagerList = _aIFarmManager?.GetComponentsInChildren<LandManager>().ToList();

        //Initializing Children Position Objects...
        _idleStateStartingPosition = GameObject.Find("IdleStatePosition");
        _farmStateStartingPosition = GameObject.Find("FarmStatePosition");
        _patrolStateStartingPosition = GameObject.Find("PatrolStatePosition");
        _interactStateStartingPosition = GameObject.Find("FarmStatePosition");

        //Initializing waypoints for patrol state...
        _patrolStateWaypointsList = _patrolStateStartingPosition.transform.GetComponentsInChildren<Transform>().ToList();

        //Initializing the interactor of corp indus character...
        _corpIndusInteractor = this.transform.GetChild(0).gameObject;

        //A big check for all the required varibales are loaded or not...
        if (_cIIdleState == null || _cIPatrolState == null || _cIFarmState == null || _cITalkInteractState == null)
        {
            Debug.LogError("States initialization is missing in " + this);
        }
        if(_navMeshAgent == null || _aIFarmManager == null || _aIPlotManagerList == null || _aILandManagerList == null 
            || _idleStateStartingPosition == null || _farmStateStartingPosition == null 
            || _patrolStateStartingPosition == null || _interactStateStartingPosition == null)
        {
            Debug.LogError("Objects & Lists initialization is missing in " + this);
        }

        _aiLight = transform.GetChild(2).GetComponent<Light>();

        if (_aiLight == null)
        {
            Debug.LogError("_aiAreaLight is null in " + this);
        }

        //Add to iTimer listener list...
        TimeManager.Instance.AddAsITimerListener(this);

        //Setting the default current state as Idle...
        _currentCIState = _cIIdleState; /*These don't have to be type cast explicitly as these are base abstract class and derived class, respectively*/

        //Passing state manager instance to current state...
        _currentCIState.EnterState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //When collision happens, we update the OnCollisionEnter method in the concrete states...
        _currentCIState.CollisionEnter(this, collision);
    }

    // Update is called once per frame
    void Update()
    {
        //CheckIfTimeToCallTheDay();

        //Updating the UpdateState method in the concrete states every frame...
        _currentCIState.UpdateState(this);

        _currentState = _currentCIState.ToString();

        if(_currentState == "CorpIndusIdleState")
        {
            _aiLight.color = Color.yellow;
        }
        else if (_currentState == "CorpIndusPatrolState")
        {
            _aiLight.color = Color.red;
        }
        else if(_currentState == "CorpIndusFarmState")
        {
            _aiLight.color = Color.green;
        }
        else
        {
            _aiLight.color = Color.white;
        }
    }

    bool CheckIfTimeToCallTheDay()
    {
        if (TimeManager.Instance.GetCurrentPartOfTheDay() == "Night" || TimeManager.Instance.GetCurrentPartOfTheDay() == "MidNight")
        {
            return true;
        }

        return false;
    }

    private void SwitchState(CorpIndusAbstractBaseState state)
    {
        _currentCIState = state;
        _currentCIState.EnterState(this);
    }

    public void TickUpdate(GameTimer gameTimer)
    {
        //Initializing _gameTimer with Game Timer Instance...
        if(_gameTimer == null) { _gameTimer = gameTimer; }
    }
}