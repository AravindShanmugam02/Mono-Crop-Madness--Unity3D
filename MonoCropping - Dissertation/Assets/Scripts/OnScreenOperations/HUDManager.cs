using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public void AddAsIHUDListener(iHUDInfo listener) { RegisterAsIHUDListener(listener); }
    public void RemoveFromIHUDListener(iHUDInfo listener) { UnregisterFromIHUDListeners(listener); }
    public bool IsInteracting() { return _isInteracting; }
    public bool IsBarnOpen() { return _isBarnOpen; }
    public bool IsGrainsAndSeedsStorageOpen() { return _isGrainsAndSeedsStorageOpen; }
    public bool IsShopOpen() { return _isShopOpen; }
    public bool IsInventoryOpen() { return _isInventoryOpen; }
    public bool IsModalScreenOpen() { return _isModalScreenOpen; }
    public bool IsFeedbackActive() { return _isFeedbackActive; }
    public bool IsPaused() { return _isPaused; } //Had problem with mouse cursor when timescale set to 0,
                                                 //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                                 //Now it is fixed by moveing to Update() of Mouse Manager.

    public float GetOverallNutritionLevelOfAllPlots() { return _overallNutritionLevelOfAllPlots; }
    public float GetOverallHealthLevelOfAllPlots() { return _overallHealthLevelOfAllPlots; }
    public string GetAverageFarmingPatternOfLands() { return _averageFarmingPatternOfLands; }
    public string GetMostOfLastCultivatedCropsOfLands() { return _mostOfLastCultivatedCropsOfLands; }
    public string GetAverageFarmingPatternFromPlots() { return _averageFarmingPatternFromPlots; }
    public float GetOverallEnvironmentHealthLevel() { return _overallEnvironmentHealthLevel; }
    public float GetOverallCommunityLevel() { return _overallCommunityLevel; }
    public float GetOverallCommunityNutritionLevel() { return _overallCommunityNutritionLevel; }
    public float GetOverallCommunityPopulationPercentage() { return _overallCommunityPopulationPercentage; }
    public int GetPopulationCount() { return _populationCount; }
    public int GetMaxPopulationCount() { return _maxPopulationCount; }
    public int GetOverallFoodCount() { return _foodCount; }
    public int GetOverallStorageCount() { return _storageCount; }
    public int GetBarnFoodItemsTargetCount() { return _foodTargetCount; }
    public int GetBarnFoodItemsSafeCount() { return _foodSafeCount; }
    public int GetToolsCount() { return _toolsCount; }
    public int GetEquipmentsCount() { return _equipmentsCount; }
    public int GetCropsCount() { return _cropsCount; }
    public int GetSeedsCount() { return _seedsCount; }


    private InteractOptionsManager _interactOptionsManager;
    private BarnManager _barnManager;
    private GrainsAndSeedsStorageManager _grainsAndSeedsStorageManager;
    private GreenHouseBuildingManager _greenHouseBuildingManager;
    private LandManagerHUD _landManagerHUD;
    private ShopManager _shopManager;
    private InventoryManager _inventoryManager;
    private InGameModalScreenManager _inGameModalScreenManager;
    private FeedbackManager _feedbackManager;
    private PauseManager _pauseManager; //Had problem with mouse cursor when timescale set to 0,
                                        //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                        //Now it is fixed by moveing to Update() of Mouse Manager.

    private EnvironmentHealthManager _environmentHealthManager;
    private CommunityManager _communityManager;
    private List<PlotManager> _plotManagerList;

    [SerializeField] private bool _isInteracting;
    [SerializeField] private bool _isBarnOpen;
    [SerializeField] private bool _isGrainsAndSeedsStorageOpen;
    [SerializeField] private bool _isGreenhouseOpen;
    [SerializeField] private bool _isLandDetailsViewed;
    [SerializeField] private bool _isShopOpen;
    [SerializeField] private bool _isInventoryOpen;
    [SerializeField] private bool _isModalScreenOpen;
    [SerializeField] private bool _isFeedbackActive;
    [SerializeField] private bool _isPaused; //Had problem with mouse cursor when timescale set to 0,
                                             //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                             //Now it is fixed by moveing to Update() of Mouse Manager.

    [SerializeField] private int _foodCount;
    [SerializeField] private int _storageCount;
    [SerializeField] private int _foodTargetCount;
    [SerializeField] private int _foodSafeCount;

    [SerializeField] private int _seedsCount;
    [SerializeField] private int _cropsCount;
    [SerializeField] private int _toolsCount;
    [SerializeField] private int _equipmentsCount;

    [SerializeField] private int _populationCount;
    [SerializeField] private int _maxPopulationCount;

    [SerializeField] private float _overallNutritionLevelOfAllPlots;
    [SerializeField] private float _overallHealthLevelOfAllPlots;

    [SerializeField] private float _overallEnvironmentHealthLevel;

    [SerializeField] private float _overallCommunityLevel;
    [SerializeField] private float _overallCommunityNutritionLevel;
    [SerializeField] private float _overallCommunityPopulationPercentage;

    [SerializeField] private List<string> _overallFarmingPatternOfLandsFromEachPlot;
    [SerializeField] private List<string> _mostOfLastCultivatedCropFromLandsFromEachPlot;
    [SerializeField] private List<string> _overallFarmingPatternOfPlots;

    private ConcurrentDictionary<string, int> _countOfSameFarmingPatternsFromLands;
    private ConcurrentDictionary<string, int> _countOfMostOfLastCultivatedCropFromLands;
    private ConcurrentDictionary<string, int> _countOfSameFarmingPatternsFromPlots;
    private int _maxNumberOfSameitems;

    [SerializeField] private string _averageFarmingPatternOfLands;
    [SerializeField] private string _mostOfLastCultivatedCropsOfLands;
    [SerializeField] private string _averageFarmingPatternFromPlots;

    //iHUD Interface List
    List<iHUDInfo> _iHUDListenersList;

    public static HUDManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _iHUDListenersList = new List<iHUDInfo>();

        _interactOptionsManager = GameObject.Find("InteractOptionsManager").GetComponent<InteractOptionsManager>();
        _barnManager = GameObject.Find("OldBarn").GetComponent<BarnManager>();
        _grainsAndSeedsStorageManager = GameObject.Find("GrainsAndSeedsStorage").GetComponent<GrainsAndSeedsStorageManager>();
        _greenHouseBuildingManager = GameObject.Find("GreenHouseManager").GetComponent<GreenHouseBuildingManager>();
        _landManagerHUD = GameObject.Find("LandManagerHUD").GetComponent<LandManagerHUD>();
        _shopManager = GameObject.Find("ShopManager").GetComponent<ShopManager>();
        _inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        _inGameModalScreenManager = GameObject.Find("InGameModalScreenManager").GetComponent<InGameModalScreenManager>();
        _feedbackManager = GameObject.Find("FeedbackManager").GetComponent<FeedbackManager>();
        _pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>(); //Had problem with mouse cursor when timescale set to 0,
                                                                                      //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                                                                      //Now it is fixed by moveing to Update() of Mouse Manager.
        _environmentHealthManager = GameObject.Find("EnvironmentHealthManager").GetComponent<EnvironmentHealthManager>();
        _communityManager = GameObject.Find("CommunityManager").GetComponent<CommunityManager>();

        _plotManagerList = new List<PlotManager>();

        for (int j = GameObject.Find("FarmManager").transform.childCount, i = 0; i < j; i++)
        {
            _plotManagerList.Add(GameObject.Find("FarmManager").transform.GetChild(i).GetComponent<PlotManager>());
        }

        _overallFarmingPatternOfLandsFromEachPlot = new List<string>();
        _mostOfLastCultivatedCropFromLandsFromEachPlot = new List<string>();
        _overallFarmingPatternOfPlots = new List<string>();

        _countOfSameFarmingPatternsFromLands = new ConcurrentDictionary<string, int>();
        _countOfMostOfLastCultivatedCropFromLands = new ConcurrentDictionary<string, int>();
        _countOfSameFarmingPatternsFromPlots = new ConcurrentDictionary<string, int>();

        PerformNullChecks();
    }

    //Perform null checks...
    void PerformNullChecks()
    {
        if (_interactOptionsManager == null)
        {
            Debug.LogError("_interactOptionsManager is null in HUD Manager!");
        }

        if (_barnManager == null)
        {
            Debug.LogError("_barnManager is null in HUD Manager!");
        }

        if (_grainsAndSeedsStorageManager == null)
        {
            Debug.LogError("_grainsAndSeedsStorageManager is null in HUD Manager!");
        }

        if (_greenHouseBuildingManager == null)
        {
            Debug.LogError("_greenHouseBuildingManager is null in HUD Manager!");
        }

        if (_landManagerHUD == null)
        {
            Debug.LogError("_landManagerHUD is null in HUD Manager!");
        }

        if (_shopManager == null)
        {
            Debug.LogError("_shopManager is null in HUD Manager!");
        }

        if (_inventoryManager == null)
        {
            Debug.LogError("_inventoryManager is null in HUD Manager!");
        }

        if (_inGameModalScreenManager == null)
        {
            Debug.LogError("_modalScreenManager is null in HUD Manager!");
        }

        if(_feedbackManager == null)
        {
            Debug.LogError("_feedbackManager is null in HUD Manager!");
        }

        //Had problem with mouse cursor when timescale set to 0,
        //because mouse lock was updated in FixedUpdate in Mouse Manager.
        //Now it is fixed by moveing to Update() of Mouse Manager.
        if (_pauseManager == null)
        {
            Debug.LogError("_pauseManager is null in HUD Manager!");
        }

        if (_communityManager == null)
        {
            Debug.LogError("_communityManager is null in HUD Manager!");
        }

        if (_environmentHealthManager == null)
        {
            Debug.LogError("_environmentHealthManager is null in HUD Manager!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _isInteracting = false;
        _isBarnOpen = false;
        _isGrainsAndSeedsStorageOpen = false;
        _isGreenhouseOpen = false;
        _isLandDetailsViewed = false;
        _isShopOpen = false;
        _isInventoryOpen = false;
        _isModalScreenOpen = false;
        _isFeedbackActive = false;
        _isPaused = false; //Had problem with mouse cursor when timescale set to 0,
                           //because mouse lock was updated in FixedUpdate in Mouse Manager.
                           //Now it is fixed by moveing to Update() of Mouse Manager.

        _foodCount = 0;
        _storageCount = 0;
        _foodTargetCount = 0;
        _foodSafeCount = 0;

        _seedsCount = 0;
        _cropsCount = 0;
        _toolsCount = 0;
        _equipmentsCount = 0;

        _populationCount = 0;

        _overallNutritionLevelOfAllPlots = 0f;
        _overallHealthLevelOfAllPlots = 0f;

        _overallEnvironmentHealthLevel = 0f;

        _overallCommunityLevel = 0f;
        _overallCommunityNutritionLevel = 0f;
        _overallCommunityPopulationPercentage = 0f;

        _averageFarmingPatternOfLands = "";
        _averageFarmingPatternFromPlots = "";

        ResetMaxVaribale();
    }

    private void Update()
    {
        ResetListsAndDictionaries();

        ResetFloatsAndInts();

        FetchBoolValuesFromRespectiveClasses();

        FetchAllValuesFromPlotManager();

        FetchAllValuesFromEnvironmentHealthManager();

        FetchAllValuesFromCommunityManager();

        FetchAllValuesFromPopulationCountManager();

        FetchAllValuesFromBarnManager();

        FetchAllValuesFromInventoryManager();

        ResetMaxVaribale();

        GetAverageFarmingPatternsOfLands();

        ResetMaxVaribale();

        GetMostOfLastCultivatedCropFromLandsFromEachPlot();

        ResetMaxVaribale();

        GetAverageFarmingPatternsOfPlots();

        DistributeInfoToListeners();
    }

    //Reset Lists and Dictionaries...
    void ResetListsAndDictionaries()
    {
        _overallFarmingPatternOfLandsFromEachPlot.Clear();
        _mostOfLastCultivatedCropFromLandsFromEachPlot.Clear();
        _overallFarmingPatternOfPlots.Clear();

        _countOfSameFarmingPatternsFromLands.Clear();
        _countOfMostOfLastCultivatedCropFromLands.Clear();
        _countOfSameFarmingPatternsFromPlots.Clear();
    }

    //Reset Floats and Ints...
    void ResetFloatsAndInts()
    {
        _foodCount = 0;
        _storageCount = 0;
        _foodTargetCount = 0;
        _foodSafeCount = 0;

        _seedsCount = 0;
        _cropsCount = 0;
        _toolsCount = 0;
        _equipmentsCount = 0;

        _populationCount = 0;

        _overallEnvironmentHealthLevel = 0f;

        _overallNutritionLevelOfAllPlots = 0f;
        _overallHealthLevelOfAllPlots = 0f;

        _overallCommunityLevel = 0f;
        _overallCommunityNutritionLevel = 0f;
        _overallCommunityPopulationPercentage = 0f;
    }

    //Fetch bool values from respective classes...
    void FetchBoolValuesFromRespectiveClasses()
    {
        _isInteracting = _interactOptionsManager.GetInteractionStatus();
        _isBarnOpen = _barnManager.GetBarnOpenStatus();
        _isGrainsAndSeedsStorageOpen = _grainsAndSeedsStorageManager.GetGrainsAndSeedsStorageOpenStatus();
        _isGreenhouseOpen = _greenHouseBuildingManager.GetIsGreenhouseOpen();
        _isLandDetailsViewed = _landManagerHUD.GetIsLandDetailsViewed();
        _isShopOpen = _shopManager.GetShopOpenStatus();
        _isInventoryOpen = _inventoryManager.GetInventoryStatus();
        _isModalScreenOpen = _inGameModalScreenManager.IsInGameModalScreenActive();
        _isFeedbackActive = _feedbackManager.GetIsFeedbackActive();
        _isPaused = _pauseManager.GetPausedStatus(); //Had problem with mouse cursor when timescale set to 0,
                                                     //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                                     //Now it is fixed by moveing to Update() of Mouse Manager.
    }

    //Fetch all values from Plot Manager...
    void FetchAllValuesFromPlotManager()
    {
        foreach (var item in _plotManagerList)
        {
            _overallNutritionLevelOfAllPlots += item.GetOverallPlotNutritionLevel();
            _overallHealthLevelOfAllPlots += item.GetOverallPlotHealth();

            _overallFarmingPatternOfLandsFromEachPlot.Add(item.GetAveragePatternOfChildrenLand());
            _mostOfLastCultivatedCropFromLandsFromEachPlot.Add(item.GetMostOfLastCultivatedCropFromChildrenLand());

            _overallFarmingPatternOfPlots.Add(item.GetFarmingPatternOfPlot());
        }
        _overallNutritionLevelOfAllPlots /= _plotManagerList.Count;
        _overallHealthLevelOfAllPlots /= _plotManagerList.Count;
    }

    //Fetch all values from environment health manager...
    void FetchAllValuesFromEnvironmentHealthManager()
    {
        _overallEnvironmentHealthLevel = _environmentHealthManager.GetEnvironmentHealthLevel();
    }

    //Fetch all values from community manager...
    void FetchAllValuesFromCommunityManager()
    {
        _overallCommunityLevel = _communityManager.GetOverallCommunityLevel();
        _overallCommunityNutritionLevel = _communityManager.GetOverallCommunityNutritionLevel();
        _overallCommunityPopulationPercentage = _communityManager.GetOverallCommunityPopulationPercentage();
    }

    //Fetch all values from Population Count Manager...
    void FetchAllValuesFromPopulationCountManager()
    {
        _populationCount = PopulationCountManager.Instance.GetCurrentPopulationCount();
        _maxPopulationCount = PopulationCountManager.Instance.GetMaxPopulationCount();
    }

    //Fetch storage values from Barn Manager...
    void FetchAllValuesFromBarnManager()
    {
        _foodCount = _barnManager.GetStorageFoodItemsCount();
        _storageCount = _barnManager.GetStorageItemsCount();
        _foodTargetCount = _barnManager.GetStorageFoodItemsTargetCount();
        _foodSafeCount = _barnManager.GetStorageFoodItemsSafeCount();
    }

    //Fetch Inventory items count from Inventory Manager...
    void FetchAllValuesFromInventoryManager()
    {
        _seedsCount = _inventoryManager.GetNumberOfSeedsInInventory();
        _cropsCount = _inventoryManager.GetNumberOfCropsInInventory();
        _toolsCount = _inventoryManager.GetNumberOfToolsInInventory();
        _equipmentsCount = _inventoryManager.GetNumberOfEquipmentsInInventory();
    }

    //Reset _maxNumberOfSameitems variable...
    void ResetMaxVaribale()
    {
        _maxNumberOfSameitems = int.MinValue;
    }

    //Get Average Farming Pattern Of Lands...
    void GetAverageFarmingPatternsOfLands()
    {
        //Lands Patterns
        for (int i = 0; i < _overallFarmingPatternOfLandsFromEachPlot.Count; i++)
        {
            _countOfSameFarmingPatternsFromLands.AddOrUpdate(_overallFarmingPatternOfLandsFromEachPlot[i], 1, (key, value) => value + 1);
        }
        //Lands Patterns
        foreach (var item in _countOfSameFarmingPatternsFromLands)
        {
            if (item.Value >= _maxNumberOfSameitems)
            {
                _maxNumberOfSameitems = item.Value;
                _averageFarmingPatternOfLands = item.Key;
            }
        }
    }

    //Get Most Of Last Cultivated Crop From Lands From Each Plot...
    void GetMostOfLastCultivatedCropFromLandsFromEachPlot()
    {
        //Cultivated Crops
        for (int i = 0; i < _mostOfLastCultivatedCropFromLandsFromEachPlot.Count; i++)
        {
            _countOfMostOfLastCultivatedCropFromLands.AddOrUpdate(_mostOfLastCultivatedCropFromLandsFromEachPlot[i], 1, (key, value) => value + 1);
        }
        //Cultivated Crops
        foreach (var item in _countOfMostOfLastCultivatedCropFromLands)
        {
            if (item.Value >= _maxNumberOfSameitems)
            {
                _maxNumberOfSameitems = item.Value;
                _mostOfLastCultivatedCropsOfLands = item.Key;
            }
        }
    }

    //Get Average Farming Pattern Of Plots...
    void GetAverageFarmingPatternsOfPlots()
    {
        //Plots Patterns
        for (int i = 0; i < _overallFarmingPatternOfPlots.Count; i++)
        {
            _countOfSameFarmingPatternsFromPlots.AddOrUpdate(_overallFarmingPatternOfPlots[i], 1, (key, value) => value + 1);
        }
        //Plots Patterns
        foreach (var item in _countOfSameFarmingPatternsFromPlots)
        {
            if (item.Value >= _maxNumberOfSameitems)
            {
                _maxNumberOfSameitems = item.Value;
                _averageFarmingPatternFromPlots = item.Key;
            }
        }
    }

    //Distribute info to listeners...
    void DistributeInfoToListeners()
    {
        foreach (var item in _iHUDListenersList)
        {
            item.HUDInfoUpdate(_isInteracting, _isBarnOpen, _isGrainsAndSeedsStorageOpen, _isGreenhouseOpen, _isLandDetailsViewed, 
                _isShopOpen, _isInventoryOpen, _isModalScreenOpen, _isFeedbackActive, _isPaused); //Had problem with mouse cursor when timescale set to 0,
                                                                                                  //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                                                                                  //Now it is fixed by moveing to Update() of Mouse Manager.
        }
    }

    //Handling registering as listener.
    void RegisterAsIHUDListener(iHUDInfo listener)
    {
        _iHUDListenersList.Add(listener);
    }

    //Handling Unregistering from listeners.
    void UnregisterFromIHUDListeners(iHUDInfo listener)
    {
        _iHUDListenersList.Remove(listener);
    }
}