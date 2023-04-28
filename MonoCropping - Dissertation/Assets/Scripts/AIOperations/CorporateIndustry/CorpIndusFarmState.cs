using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

[System.Serializable]
public class CorpIndusFarmState : CorpIndusAbstractBaseState
{
    //Public Getters and Setters...
    public SeedItem GetSeetItem1() { return _seedItem1; }
    public SeedItem GetSeetItem2() { return _seedItem2; }
    public void SetCultivationStatus(bool toggle) { _isCultivationCompleted = toggle; }
    public void SetWateringStatus(bool toggle) { _isWateringCompleted = toggle; }
    public void SetHarvestingStatus(bool toggle) { _isHarvestingCompleted = toggle; }
    public bool GetCultivationStatus() { return _isCultivationCompleted; }
    public bool GetWateringStatus() { return _isWateringCompleted; }
    public bool GetHarvestingStatus() { return _isHarvestingCompleted; }
    public Dictionary<PlotManager, SeedItem> GetAIPlotsWithRespectiveCrops() { return _aIPlotsWithRespectiveCropsToCultivate; }


    //Handle for Game Timer...
    private GameTimer _gameTimer;
    private GameTimer _ownFarmStateTimer;

    //Handle for Nav Mesh Agent...
    private NavMeshAgent _navMeshAgent;

    //Handle for list of AI Land Managers and Plot Managers...
    private List<LandManager> _landManagersList;
    private List<PlotManager> _plotManagersList;

    //Handle for Dictionaries of AI lands that needs to be Watered, Cultivated or Harvested...
    private Dictionary<LandManager, Vector3> _aILandsThatNeedsToBeCultivated;
    private Dictionary<LandManager, Vector3> _aILandsThatNeedsToBeWatered;
    private Dictionary<LandManager, Vector3> _aILandsThatNeedsToBeHarvested;

    //Handle for Dictionaries of AI plots that are mapped with specific crop to be planted...
    private Dictionary<PlotManager, SeedItem> _aIPlotsWithRespectiveCropsToCultivate;

    //Handle for Seed Items...
    [SerializeField] private SeedItem _seedItem1;
    [SerializeField] private SeedItem _seedItem2;
    //[SerializeField] private List<SeedItem> _seedList;

    //Sub State handle to hold current sub state information...
    private CorpIndusAbstractFarmSubState _currentCISubState;

    //Handles for different sub states of CorporateIndustry...
    [NonSerialized] public CorpIndusFarmStateCultivateSubState _cIFarmSubStateCultivateState;
    [NonSerialized] public CorpIndusFarmStateWaterSubState _cIFarmSubStateWaterState;
    [NonSerialized] public CorpIndusFarmStateHarvestSubState _cIFarmSubStateHarvestState;

    //current sub state indicator in string...
    [SerializeField] private string _currentSubState;

    //Handles coroutine running frequency...
    [SerializeField] private bool _coroutineRanOnce = false;
    [SerializeField] private bool _dataLoadedForCoroutine = false;

    //Handles for flow between sub states...
    [SerializeField] private bool _isCultivationCompleted = false;
    [SerializeField] private bool _isWateringCompleted = false;
    [SerializeField] private bool _isHarvestingCompleted = false;

    [SerializeField] private bool _updateSubState = false;

    [SerializeField] private int _numOfLandsNeedsToBeCultivated;
    [SerializeField] private int _numOfLandsNeedsToBeWatered;
    [SerializeField] private int _numOfLandsNeedsToBeHarvested;

    public override void EnterState(CorpIndusStateManager corpIndusStateManager)
    {
        _gameTimer = corpIndusStateManager.GetGameTimer();
        _ownFarmStateTimer = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);

        _navMeshAgent = corpIndusStateManager.GetNavMeshAgent();
        _navMeshAgent.SetDestination(corpIndusStateManager.GetFarmStateStartingPosition().position);

        if (_landManagersList == null || _landManagersList.Count == 0)
        {
            _landManagersList = new List<LandManager>();
            _landManagersList = corpIndusStateManager.GetAILandManagerList();
        }

        if(_plotManagersList == null || _plotManagersList.Count == 0)
        {
            _plotManagersList = new List<PlotManager>();
            _plotManagersList = corpIndusStateManager.GetAIPlotManagerList();
        }

        _seedItem1 = corpIndusStateManager.GetSeedItem1();
        _seedItem2 = corpIndusStateManager.GetSeedItem2();

        //if (_seedItem1 == null || _seedItem2 == null)
        //{
        //    _seedItem1 = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Sugarcane Seed.asset", typeof(SeedItem)) as SeedItem;
        //    _seedItem2 = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Cotton Seed.asset", typeof(SeedItem)) as SeedItem;
        //    //[LATER SHOULD TRY TO LOAD THE SEEDITEMS IN WHOLE AS AN ARRAY OR LIST. SO THAT I COULD ADD IT EASILY IN THE DICTIONARIES WITH PLOTMANAGER]
        //}

        _aILandsThatNeedsToBeCultivated = new Dictionary<LandManager, Vector3>();
        _aILandsThatNeedsToBeWatered = new Dictionary<LandManager, Vector3>();
        _aILandsThatNeedsToBeHarvested = new Dictionary<LandManager, Vector3>();

        _aIPlotsWithRespectiveCropsToCultivate = new Dictionary<PlotManager, SeedItem>();
        _aIPlotsWithRespectiveCropsToCultivate.Add(_plotManagersList[_plotManagersList.Count - 2], _seedItem1/*_seedList[0]*/);
        _aIPlotsWithRespectiveCropsToCultivate.Add(_plotManagersList[_plotManagersList.Count - 1], _seedItem2/*_seedList[1]*/);

        if (_cIFarmSubStateCultivateState == null) { _cIFarmSubStateCultivateState = new CorpIndusFarmStateCultivateSubState(); }
        if (_cIFarmSubStateWaterState == null) { _cIFarmSubStateWaterState = new CorpIndusFarmStateWaterSubState(); }
        if (_cIFarmSubStateHarvestState == null) { _cIFarmSubStateHarvestState = new CorpIndusFarmStateHarvestSubState(); }

        _isCultivationCompleted = false;
        _isWateringCompleted = false;
        _isHarvestingCompleted = false;

        _currentCISubState = null;
        _coroutineRanOnce = false;
        _dataLoadedForCoroutine = false;

        _numOfLandsNeedsToBeCultivated = 0;
        _numOfLandsNeedsToBeWatered = 0;
        _numOfLandsNeedsToBeHarvested = 0;
    }

    public override void CollisionEnter(CorpIndusStateManager corpIndusStateManager, Collision collision)
    {

    }

    public override void UpdateState(CorpIndusStateManager corpIndusStateManager)
    {
        if (_aILandsThatNeedsToBeCultivated.Count == 0 && _aILandsThatNeedsToBeWatered.Count == 0 && _aILandsThatNeedsToBeHarvested.Count == 0)
        {
            if (!corpIndusStateManager.ShouldWeCallForTheDay())
            {
                foreach (var item in _landManagersList)
                {
                    //This is the part where the lands that needs to be cultivated will be decided for each...
                    //item.GetIsLandCultivated() && item.GetLandState() == LandManager.LandState.Dry -- Is because if it raisna nd becomes clay, then it becomes dry.
                    //The cultivated crops will be still there. So to destroy them during cultivation process.
                    if (!item.GetIsLandCultivated() || (item.GetIsLandCultivated() && item.GetLandState() == LandManager.LandState.Dry))
                    {
                        _aILandsThatNeedsToBeCultivated.Add(item, item.transform.position);
                    }

                    //This is the part where the lands that needs to be watered will be decided for each...
                    if (item.GetIsLandCultivated() && item.GetLandState() == LandManager.LandState.Muddy)
                    {
                        _aILandsThatNeedsToBeWatered.Add(item, item.transform.position);
                    }

                    //This is the part where the lands that needs to be harvested will be decided for each...
                    if (item.GetAIHarvestStatus())
                    {
                        _aILandsThatNeedsToBeHarvested.Add(item, item.transform.position);
                    }
                }
            }

            _dataLoadedForCoroutine = true; //Even after end of the day, data will be loaded, but it will be 0.
        }

        _numOfLandsNeedsToBeCultivated = _aILandsThatNeedsToBeCultivated.Count;
        _numOfLandsNeedsToBeWatered = _aILandsThatNeedsToBeWatered.Count;
        _numOfLandsNeedsToBeHarvested = _aILandsThatNeedsToBeHarvested.Count;

        if (_coroutineRanOnce == false && _dataLoadedForCoroutine == true)
        {
            corpIndusStateManager.StartCoroutine(FarmCoroutine(corpIndusStateManager));
        }

        UpdateSubState(corpIndusStateManager);

        _currentSubState = _currentCISubState?.ToString();
    }

    void SwitchSubStates(CorpIndusStateManager corpIndusStateManager, CorpIndusAbstractFarmSubState subState, string subStateInString)
    {
        _currentCISubState = subState;

        if(subStateInString == "Cultivate")
        {
            if(_aILandsThatNeedsToBeCultivated.Count == 0)
            {
                //Debug.Log("There are no lands that needs to be cultivated!");

                _isCultivationCompleted = true;
                _updateSubState = false;
            }
            else
            {
                _currentCISubState.EnterSubState(corpIndusStateManager, this, _aILandsThatNeedsToBeCultivated);
                _updateSubState = true;
            }
        }
        else if(subStateInString == "Water")
        {
            if (_aILandsThatNeedsToBeWatered.Count == 0)
            {
                //Debug.Log("There are no lands that needs to be watered!");

                _isWateringCompleted = true;
                _updateSubState = false;
            }
            else
            {
                _currentCISubState.EnterSubState(corpIndusStateManager, this, _aILandsThatNeedsToBeWatered);
                _updateSubState = true;
            }
        }
        else if(subStateInString == "Harvest")
        {
            if (_aILandsThatNeedsToBeHarvested.Count == 0)
            {
                //Debug.Log("There are no lands that needs to be harvested!");
                _isHarvestingCompleted = true;
                _updateSubState = false;
            }
            else
            {
                _currentCISubState.EnterSubState(corpIndusStateManager, this, _aILandsThatNeedsToBeHarvested);
                _updateSubState = true;
            }
        }
    }

    void UpdateSubState(CorpIndusStateManager corpIndusStateManager)
    {
        if (_currentCISubState != null && _updateSubState == true)
        {
            _currentCISubState.UpdateSubState(corpIndusStateManager, this);
        }
    }

    IEnumerator FarmCoroutine(CorpIndusStateManager corpIndusStateManager)
    {
        _coroutineRanOnce = true;

        //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
        yield return new WaitUntil(CheckWeatherNavMeshAgentHasReachedDestination);

        SwitchSubStates(corpIndusStateManager, _cIFarmSubStateCultivateState, "Cultivate");

        yield return new WaitUntil(GetCultivationStatus);

        SwitchSubStates(corpIndusStateManager, _cIFarmSubStateWaterState, "Water");

        yield return new WaitUntil(GetWateringStatus);

        SwitchSubStates(corpIndusStateManager, _cIFarmSubStateHarvestState, "Harvest");

        yield return new WaitUntil(GetHarvestingStatus);

        _currentCISubState = null;
        _currentSubState = "null";

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
}