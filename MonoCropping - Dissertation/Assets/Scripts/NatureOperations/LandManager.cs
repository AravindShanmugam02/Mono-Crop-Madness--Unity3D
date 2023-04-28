using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

//Class Links: InteractOptionsManager
//Interface Links: InteractInfo
//GameObjects Links: Selected

public class LandManager : MonoBehaviour, iInteractInfo, InventoryInfo, ITimer, SkyInfo
{
    #region Public Getters & Setters

    public void SetEquipedItem(GameItemsData ItemCS, GameItemsData ItemTE) { _equipedItemSlotDataCS = new GameItemsSlotData(ItemCS); _equipedItemSlotDataTE = new GameItemsSlotData(ItemTE); }
    public GameItemsData GetEquipedItemCS() { return _equipedItemSlotDataCS.GetGameItemsData(); }
    public GameItemsData GetEquipedItemTE() { return _equipedItemSlotDataTE.GetGameItemsData(); }
    public SeedItem GetAICultivatedCrop() { return _aILandCultivatedCrop; }
    public bool GetAIHarvestStatus() { return AITryHarvest(); }
    public LandState GetLandState() { return _landState; }
    public LandOwnership GetLandOwnership() { return _landOwnership; }
    public bool GetIsLandCultivated() { return _isCultivated; }
    public bool GetIsLandHarvested() { return _isHarvested; }
    public string GetLastCultivated() { if (_landFarmingCropsRecord.Count == 0) { return ""; } else { return _landFarmingCropsRecord.Last(); } }
    public List<string> GetLandFarmingCropsRecord() { return _landFarmingCropsRecord; }
    public string GetLandFarmingPattern() { return _landFarmingPattern.ToString(); }
    public float GetOverallLandHealth() { return _overallLandHealth; }
    public float GetLandNutritionLevel() { return _landNutritionLevel; }
    public bool GetIsSpreadingFromThis() { return _isSpreadingFromThis; }
    public bool GetHasSpreadedFromOthers() { return _hasSpreadedFromOthers; }
    public void SetHasSpreadFromOthers(bool trigger) { _hasSpreadedFromOthers = trigger; }

    #endregion

    #region Class Members

    #region Materials And Textures Objects
    [Header("Material")]
    [SerializeField] private Material _clayGroundMat;
    [SerializeField] private Material _dryGroundMat;
    [SerializeField] private Material _farmGroundMat;
    [SerializeField] private Material _grassyGroundMat;
    [SerializeField] private Material _muddyGroundMat;

    [Header("PhysicMaterial")]
    [SerializeField] private PhysicMaterial _clayPhysicMat;
    [SerializeField] private PhysicMaterial _dryPhysicMat;
    [SerializeField] private PhysicMaterial _farmPhysicMat;
    [SerializeField] private PhysicMaterial _grassyPhysicMat;
    [SerializeField] private PhysicMaterial _muddyPhysicMat;

    [Header("Child")]
    [SerializeField] private GameObject _selected;
    #endregion

    //Plot Manager Handle...
    private PlotManager _plotManager;

    //Land Ownership...
    public enum LandOwnership
    {
        Player, AI
    }
    [SerializeField] private LandOwnership _landOwnership;

    //Land State...
    public enum LandState
    {
        Dry, Clay, Muddy, Farm, Grassy
    }
    [SerializeField] private LandState _landState;

    //Farming Commands...
    public enum PlayerInteractionCommandsForPlayerLand
    {
        Till, Water, Cultivate, Harvest, Destroy, Restore
    }
    private PlayerInteractionCommandsForPlayerLand _farmingCommandForPlayerLand;
    public enum PlayerInteractionCommandsForAILand
    {
        Till, Water, Cultivate, Destroy, Restore
    }
    private PlayerInteractionCommandsForAILand _farmingCommandsForAILand;

    //Farming Commands List...
    private List<string> _listOfFarmingCommandsForPlayerLand;
    private List<string> _listOfFarmingCommandsForAILand;

    //Land health system that counts to environment health...
    [SerializeField] private float _landStateHealth;
    [SerializeField] private float _landFarmingPatternHealth;
    [SerializeField] private float _overallLandHealth;
    [SerializeField] private float _landNutritionLevel;
    [SerializeField] private int _maxHistoryCapacity;
    [SerializeField] private List<string> _landFarmingCropsRecord; //https://stackoverflow.com/questions/11957011/what-happened-to-the-index-of-a-list-when-elements-are-add-removed
    
    //Enums for Land Farming Patterns...
    public enum LandFarmingPatterns
    {
        NoPattern, PotentialMonocropping, Monocropping, CropRotation
    }
    [SerializeField] private LandFarmingPatterns _landFarmingPattern;
    [SerializeField] private LandFarmingPatterns _previousLandFarmingPattern;
    [SerializeField] private List<LandFarmingPatterns> _landFarmingPatternRecord; //Lets use this to calculate weekly / monthly farming pattern .... [PENDING...]

    //CropsPrefab....
    [SerializeField] private GameObject _cropsPerLand;

    //Renderer and Collider for land...
    private Renderer _landRenderer;
    private Collider _landCollider;

    //To store the Crop, Seed, Tool & Equipment information...
    private GameItemsSlotData _equipedItemSlotDataCS;
    private GameItemsSlotData _equipedItemSlotDataTE;

    //To store cultivated crop information...
    private int _reqNumOfSeedsPerLandPerCultivation;
    private SeedItem _cultivatedCrop = null;
    private SeedItem _aILandCultivatedCrop = null;

    //GameTimer Handle...
    private GameTimer _gameTimer, _ownLandStateTimer, _ownLandPatternTimer;

    //Array to store the surrounding objects...
    [SerializeField] private Collider[] _surroundingLandObjects;

    //To handle player related functions...
    private PlayerController _playerController;

    //Some crucial variables...
    private float _landStateHours;
    private float _landFarmingPatternHours;
    [SerializeField] private bool _canUseLand = false;
    [SerializeField] private bool _isCultivated = false;
    [SerializeField] private bool _isHarvested = false;
    private bool _cultivateTrigger = false;
    private bool _isRaining = false;
    private bool _is6HoursNow = false;
    [SerializeField] private bool _isSpreadingFromThis = false;
    [SerializeField] private bool _hasSpreadedFromOthers = false;

    //Watering variables...
    [SerializeField] private int _quantityReqToWaterALand = 2;


    //Land Details HUD varibales...
    [SerializeField] LandManagerHUD _landManagerHUD;
    [SerializeField] GameObject _affectedByOtherLandsIndicator;
    [SerializeField] GameObject _affectedByOtherLandsIndicatorText;
    [SerializeField] GameObject _affectingOtherLandsIndicator;
    [SerializeField] GameObject _affectingOtherLandsIndicatorText;
    [SerializeField] GameObject _landManagerHUDInteractText;

    #endregion

    #region Default Functions of Monobehaviour

    void Awake()
    {
        //Setting the maximum history capacity to be 5...
        _maxHistoryCapacity = 5;

        //Setting the number of seeds required to cultivate a land to be 3...
        _reqNumOfSeedsPerLandPerCultivation = 3;

        //Setting land health to be 100.0 on awake...
        _landStateHealth = 0.0f;
        _landFarmingPatternHealth = 0.0f;
        _overallLandHealth = 0.0f;
        _landNutritionLevel = 0.0f;

        //Initializing the required lists...
        _listOfFarmingCommandsForPlayerLand = new List<string>();
        _listOfFarmingCommandsForAILand = new List<string>();
        _landFarmingCropsRecord = new List<string>();
        _landFarmingPatternRecord = new List<LandFarmingPatterns>();
        _surroundingLandObjects = new Collider[9]; //9 because this will include surreounding 8 lands and itself...

        //Setting the Land Farming Pattern to None at first...
        _landFarmingPattern = LandFarmingPatterns.NoPattern;
        //Setting the previous land farming pattern to present land farming pattern...
        _previousLandFarmingPattern = _landFarmingPattern;
    }

    // Start is called before the first frame update
    void Start()
    {
        //_cropsPerLand = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/3D/Nature/Farming/Land/CropsPerLand.prefab", typeof(GameObject)) as GameObject;
        if (_cropsPerLand == null) { Debug.LogError("Error loading CropsPerLand in LandManager"); }

        _selected = this.transform.GetChild(0).gameObject; //First child is the selected object.

        //Setting the land ownership according to the plot ownership...
        _plotManager = this.transform.parent.GetComponent<PlotManager>();
        _landOwnership = (LandOwnership)_plotManager.GetPlotOwnershipAsInt();

        _landRenderer = GetComponent<Renderer>();
        _landCollider = GetComponent<Collider>();

        _playerController = GameObject.Find("Lydia").GetComponent<PlayerController>();

        _landManagerHUD = GameObject.Find("LandManagerHUD").GetComponent<LandManagerHUD>();
        _affectedByOtherLandsIndicator = _landManagerHUD?.GetAffectedByOtherLandsIndicatorObject();
        _affectedByOtherLandsIndicatorText = _landManagerHUD?.GetAffectedByOtherLandsIndicatorTextObject();
        _affectingOtherLandsIndicator = _landManagerHUD?.GetAffectingOtherLandsIndicatorObject();
        _affectingOtherLandsIndicatorText = _landManagerHUD?.GetAffectingOtherLandsIndicatorTextObject();
        _landManagerHUDInteractText = _landManagerHUD?.GetLandManagerHUDInteractTextObject();

        if (_landManagerHUD == null || _affectedByOtherLandsIndicator == null || 
            _affectingOtherLandsIndicator == null || _landManagerHUDInteractText == null)
        {
            Debug.LogError("_landManagerHUD or _affectedByOtherLandsIndicator or _affectingOtherLandsIndicator " +
                "or _landManagerHUDInteractText is null in Land Manager!");
        }

        _affectedByOtherLandsIndicatorText.SetActive(false);
        _affectedByOtherLandsIndicator.SetActive(false);
        _affectingOtherLandsIndicatorText.SetActive(false);
        _affectingOtherLandsIndicator.SetActive(false);
        _landManagerHUDInteractText.SetActive(false);

        _equipedItemSlotDataCS = new GameItemsSlotData();
        _equipedItemSlotDataTE = new GameItemsSlotData();

        //https://stackoverflow.com/a/14971637 - //Can also use using System; in the top instead of System.Enum below.
        _listOfFarmingCommandsForPlayerLand = System.Enum.GetNames(typeof(PlayerInteractionCommandsForPlayerLand)).ToList(); //using System.Linq for .ToList().
        _listOfFarmingCommandsForAILand = System.Enum.GetNames(typeof(PlayerInteractionCommandsForAILand)).ToList();

        TimeManager.Instance.AddAsITimerListener(this);
        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);
        InventoryDisplayManager.Instance.AddAsInventoryInfoListener(this);
        SkyManager.Instance.AddAsSkyInfoListener(this);

        //Default Material to Dry
        SwitchLandStatus(LandState.Dry);

        //Keep the Selected child gameobject to be inactive as default.
        SelectedActivate(false);

        _canUseLand = false;
        _isCultivated = false;
        _isHarvested = false;

        FindSurroundingLands();
    }

    // Update is called once per frame
    void Update()
    {
        if(_ownLandStateTimer == null && _gameTimer != null)
        {
            LandStateTimeSetter();
        }
        else if(_ownLandStateTimer != null && _gameTimer != null)
        {
            LandStateTimeTracker(_landState);
        }

        UpdateLandPatternLandHealthAndLandNutritionLevel();

        if (_ownLandPatternTimer == null && _gameTimer != null)
        {
            LandFarmingPatternTimeSetter();
        }
        else if(_ownLandPatternTimer != null && _gameTimer != null)
        {
            LandFarmingPatternTimeTracker(_landFarmingPattern);
        }
    }

    #endregion

    #region Selected Land Highlighter Toggle
    public void SelectedActivate(bool toggle) //This function will be called by Player interactor for every frame.
    {
        _selected.SetActive(toggle);

        //When this land's selected tile activates, display the affecting or not affecting indicators
        if (_selected.activeSelf)
        {
            _affectedByOtherLandsIndicator.SetActive(true);
            _affectedByOtherLandsIndicatorText.SetActive(false);
            _affectingOtherLandsIndicator.SetActive(true);
            _affectingOtherLandsIndicatorText.SetActive(false);
            _landManagerHUDInteractText.SetActive(true);

            Image affected = _affectedByOtherLandsIndicator.GetComponent<Image>();
            Image affecting = _affectingOtherLandsIndicator.GetComponent<Image>();

            if (_hasSpreadedFromOthers)
            {
                affected.color = new Color(0.8396226f, 0.5425968f, 0.1386169f, 1f); //Orange
                _affectedByOtherLandsIndicatorText.SetActive(true);
            }
            else
            {
                affected.color = Color.gray;
                _affectedByOtherLandsIndicatorText.SetActive(false);
            }

            if (_isSpreadingFromThis)
            {
                affecting.color = new Color(0.8018868f, 0.1550819f, 0.1550819f, 1f); //Red
                _affectingOtherLandsIndicatorText.SetActive(true);
            }
            else
            {
                affecting.color = Color.gray;
                _affectingOtherLandsIndicatorText.SetActive(false);
            }

            if (Input.GetKey(KeyCode.V))
            {
                _landManagerHUD.FeedInData(this);
                _landManagerHUD.ToggleLandDetailsPanel(true);
            }
        }
        else
        {
            _affectedByOtherLandsIndicatorText.SetActive(false);
            _affectedByOtherLandsIndicator.SetActive(false);
            _affectingOtherLandsIndicatorText.SetActive(false);
            _affectingOtherLandsIndicator.SetActive(false);
            _landManagerHUDInteractText.SetActive(false);
            _landManagerHUD.ToggleLandDetailsPanel(false);
        }
    }
    #endregion

    #region Switch Land Status & Pattern

    //Land State...
    public void SwitchLandStatus(LandState land)
    {
        _landState = land;

        switch (_landState)
        {
            case LandState.Dry:
                _landRenderer.material = _dryGroundMat;
                _landCollider.material = _dryPhysicMat;
                //Should implement what happens when it turns to dry land - if crops are planted, they will be destroyed... like that.
                _canUseLand = false;
                break;

            case LandState.Clay: //When Ranfall occurs...
                _landRenderer.material = _clayGroundMat;
                _landCollider.material = _clayPhysicMat;
                //
                _canUseLand = false;
                break;

            case LandState.Muddy: //When tilled dry land...
                _landRenderer.material = _muddyGroundMat;
                _landCollider.material = _muddyPhysicMat;
                //
                _canUseLand = false;
                break;

            case LandState.Farm: //When watered properly...
                _landRenderer.material = _farmGroundMat;
                _landCollider.material = _farmPhysicMat;
                //
                _canUseLand = true;
                break;

            case LandState.Grassy: //When it is farm, this happens gradually sometimes...
                _landRenderer.material = _grassyGroundMat;
                _landCollider.material = _grassyPhysicMat;
                //
                _canUseLand = false;
                break;
            
            default:
                _landRenderer.material = _dryGroundMat;
                _landCollider.material = _dryPhysicMat;
                //
                _canUseLand = false;
                break;
        }

        LandStateTimeSetter(); //So that every time when land chnages its state, it's time is kept track for the next changes to happen...
    }

    //Land Farming Pattern...
    public void SwitchLandPattern(LandFarmingPatterns landFarmingPattern)
    {
        _previousLandFarmingPattern = _landFarmingPattern;

        _landFarmingPattern = landFarmingPattern;

        if (_previousLandFarmingPattern != _landFarmingPattern)
        {
            //Call function that should be called once when the land farming pattern changes...

            switch (_landFarmingPattern)
            {
                case LandFarmingPatterns.NoPattern:
                    break;
                case LandFarmingPatterns.PotentialMonocropping:
                    break;
                case LandFarmingPatterns.Monocropping:
                    TipManager.Instance.ConstructATip("One of your land has to monocropping pattern. Find, Destroy(if cultivated) & Restore them!", false);
                    break;
                case LandFarmingPatterns.CropRotation:
                    TipManager.Instance.ConstructATip("You have achieved Crop Rotation in one of your land! Keep it up!", false);
                    break;
                default:
                    break;
            }

            //_previousLandFarmingPattern = _landFarmingPattern;

            LandFarmingPatternTimeSetter(); //Kept inside if(_previousLandFarmingPattern != _landfarmingPattern) because we need to set timer only when the land pattern changes...
        }
        
        //LandFarmingPatternTimeSetter();
    }

    #endregion

    #region Player Land Interaction

    public void PlayerInteraction(string action = "")
    {
        if(this._landOwnership == LandOwnership.Player)
        {
            if(TimeManager.Instance.GetCurrentPartOfTheDay() != "Night" && TimeManager.Instance.GetCurrentPartOfTheDay() != "MidNight")
            {
                PlayerInteract(action);
            }
            else
            {
                //Debug.Log("Its Either Night Or MidNight. Not The Time For Farming!");
                TipManager.Instance.ConstructATip("Not The Time For Farming! Come back in the morning.", true);
            }
        }
        else
        {
            //Debug.Log("Wrong Land Interaction! You are in Player Land Interaction!");
        }
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        //TILL
        if(action == PlayerInteractionCommandsForPlayerLand.Till.ToString())
        {
            StartCoroutine(TillLand());
        }
        //WATER
        else if(action == PlayerInteractionCommandsForPlayerLand.Water.ToString())
        {
            StartCoroutine(WaterLand());
        }
        //CULTIVATE
        else if(action == PlayerInteractionCommandsForPlayerLand.Cultivate.ToString())
        {
            StartCoroutine(CultivateCrops());
        }
        //HARVEST
        else if(action == PlayerInteractionCommandsForPlayerLand.Harvest.ToString())
        {
            StartCoroutine(HarvestCrops());
        }
        //DESTROY
        else if(action == PlayerInteractionCommandsForPlayerLand.Destroy.ToString())
        {
            StartCoroutine(DestroyCrops());
        }
        //RESTORE
        else if(action == PlayerInteractionCommandsForPlayerLand.Restore.ToString())
        {
            StartCoroutine(RestoreLand());
        }
    }

    IEnumerator TillLand()
    {
        if (!_equipedItemSlotDataTE.CheckIsEmpty())
        {
            if(_equipedItemSlotDataTE.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Equipment)
            {
                if(_equipedItemSlotDataTE.GetGameItemsData()._itemName == "Plough")
                {
                    if (_landState == LandState.Dry)
                    {
                        yield return new WaitForSeconds(0f);

                        SwitchLandStatus(LandState.Muddy);
                    }
                    else
                    {
                        //Debug.Log("Land Cannot Be Tilled As It Is Not A Dry Land!");
                        TipManager.Instance.ConstructATip("Land Cannot Be Tilled As It Is Not A Dry Land!", true);
                    }
                }
                else
                {
                    //Debug.Log("Equiped Tool Is Not A Plough! Please Equip A Plough To Till The Land!");
                    TipManager.Instance.ConstructATip("Equiped Tool Is Not A Plough! Please Equip A Plough To Till The Land! Need new items? Look into shop!", true);
                }
            }
            else
            {
                //Debug.Log("Equiped Item Is Not A Tool!");
                TipManager.Instance.ConstructATip("Equiped Item Is Not A Tool! Need new items? Look into shop!", true);
            }
        }
        else
        {
            //Debug.Log("Tools And Equipments Slot Is Empty!");
            TipManager.Instance.ConstructATip("Tools And Equipments Slot Is Empty! Need new items? Look into shop!", true);
        }
    }

    IEnumerator WaterLand()
    {
        if (!_equipedItemSlotDataTE.CheckIsEmpty())
        {
            if (_equipedItemSlotDataTE.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
            {
                if (_equipedItemSlotDataTE.GetGameItemsData()._itemName == "Watering Can")
                {
                    if(_equipedItemSlotDataTE.GetQuantity() >= _quantityReqToWaterALand)
                    {
                        if (_landState == LandState.Muddy)
                        {
                            yield return new WaitForSeconds(0f);

                            SwitchLandStatus(LandState.Farm);

                            InventoryManager.Instance.RemoveQuantityFromEquiped(InventorySlot.InventorySlotType.TE, _quantityReqToWaterALand);
                        }
                        else
                        {
                            //Debug.Log("Land Cannot Be Watered As It Is Not A Muddy Land!");
                            TipManager.Instance.ConstructATip("Land Cannot Be Watered As It Is Not A Muddy Land!", true);
                        }
                    }
                    else
                    {
                        //Debug.Log("Watering Can Doesn't Have Enough Quantity Of Water To Water The Land!");
                        TipManager.Instance.ConstructATip("Watering Can Doesn't Have Enough Quantity Of Water To Water The Land! Try refilling from nearby well!", true);
                    }
                }
                else
                {
                    //Debug.Log("Equiped Tool Is Not A Watering Can! Please Equip A Watering Can To Water The Land!");
                    TipManager.Instance.ConstructATip("Equiped Tool Is Not A Watering Can! Please Equip A Watering Can To Water The Land! Need new items? Look into shop!", true);
                }
            }
            else
            {
                //Debug.Log("Equiped Item Is Not A Tool!");
                TipManager.Instance.ConstructATip("Equiped Item Is Not A Tool! Need new items? Look into shop!", true);
            }
        }
        else
        {
            //Debug.Log("Tools And Equipments Slot Is Empty!");
            TipManager.Instance.ConstructATip("Tools And Equipments Slot Is Empty! Need new items? Look into shop!", true);
        }
    }

    IEnumerator CultivateCrops()
    {
        //Debug.Log("IsCultivated ::" + _isCultivated + ".  CanUseLand ::" + _canUseLand);

        if (_landState == LandState.Farm && _isCultivated == false && _canUseLand == true && _equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            if (_equipedItemSlotDataCS.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed && _equipedItemSlotDataCS.GetQuantity() >= _reqNumOfSeedsPerLandPerCultivation)
            {
                Instantiate(_cropsPerLand, transform);
                _cultivatedCrop = _equipedItemSlotDataCS.GetGameItemsData() as SeedItem;

                yield return new WaitForSeconds(0f);
                //yield return new WaitForSeconds(2f); //Need this 3 secs to have the crops instantiated and let the CropBehaviour class run it's Start function before removing quantity...

                if (_landFarmingCropsRecord.Count < _maxHistoryCapacity) //To keep the history to be only upto last five farming...
                {
                    _landFarmingCropsRecord.Add(_cultivatedCrop._cropToYield._itemName);
                }
                else if (_landFarmingCropsRecord.Count >= _maxHistoryCapacity) //To keep the history to be only upto last five farming...
                {
                    _landFarmingCropsRecord.RemoveAt(0);
                    _landFarmingCropsRecord.Add(_cultivatedCrop._cropToYield._itemName);
                }

                _isCultivated = true;
                _canUseLand = false;
                _isHarvested = false;

                //Moved remove quantity code down here as the newly instaitated crops need some time to finish their start funtion in CropBehaviour class...
                InventoryManager.Instance.RemoveQuantityFromEquiped(InventorySlot.InventorySlotType.CS, _reqNumOfSeedsPerLandPerCultivation); //Removig quantity from Inventory...
            }
            else
            {
                //Debug.Log("Not A Seed Or Not Enough Seeds To Cultivate!!!");
                TipManager.Instance.ConstructATip("Not A Seed Or Not Enough Seeds To Cultivate! You require 3 seeds to cultivate!", true);
            }
        }
        else
        {
            //Debug.Log("Command didn't process. Crops Cannot Be Cultivated!!!");
            TipManager.Instance.ConstructATip("Crops Cannot Be Cultivated! Land might not be farm land or land is already cultivated or you have not equiped any thing to cultivate!", true);

        }
    }

    IEnumerator HarvestCrops()
    {
        CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>();

        //Debug.Log("HarvestCrops ---> cropsPerLand:: " + cropsPerLand + "    isCultivated:: " + _isCultivated + "    canUseLand:: " + _canUseLand);
        
        //As per this condition, if we want to harvest the land should be farm type or muddy type...
        if((_landState == LandState.Farm || _landState == LandState.Muddy) && (_isCultivated == true /*&& _canUseland == false*/ && cropsPerLand != null))
        {
            cropsPerLand.TryHarvest();

            if(cropsPerLand.GetReadyToHarvest())
            {
                //We get the CropItem from the SeedItem and store it as GameItemData. This is because when trying to add to inventory it required a GameItemsData which is a crop not seed.
                GameItemsData gameItemsCropData = _cultivatedCrop._cropToYield;

                //AddObjectsToInventory() returns the number of items that is remaining in the cultivated land after adding the harvested items to inventory...
                int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(gameItemsCropData, cropsPerLand.GetQuantityOfItems());

                //Debug.Log("Remaining Quantity 2 ::: " + remainingQuantity);

                if(remainingQuantity >= cropsPerLand.GetQuantityOfItems())
                {
                    TipManager.Instance.ConstructATip("Inventory is full. Make some space and try again!", true);
                }

                if(remainingQuantity > 0 && remainingQuantity < cropsPerLand.GetQuantityOfItems())
                {
                    TipManager.Instance.ConstructATip("Only a portion of crops is harvested as there is not enough space for all crops from this land in inventory!", true);
                }

                cropsPerLand.RemoveQuantity(cropsPerLand.GetQuantityOfItems() - remainingQuantity);

                yield return new WaitForSeconds(0f);

                if (cropsPerLand.GetQuantityOfItems() <= 0)
                {
                    _isCultivated = false;
                    _canUseLand = true;
                    _cultivatedCrop = null;

                    cropsPerLand.SetCropsPerLandDestroyTrigger(true);

                    _isHarvested = true;
                    TipManager.Instance.ConstructATip("Harvested completely!", true);
                }
            }
            else
            {
                //Debug.Log("Not Ready To Harvest!!!");
                TipManager.Instance.ConstructATip("Land Is Not Ready To Harvest!", true);
            }
        }
        else
        {
            //Debug.Log("Check The Land State Before Harvesting!!!");
            //Debug.Log("Command didn't process. Crops Cannot Be Harvested!!!");
            TipManager.Instance.ConstructATip("Either the land is not in farm state or muddy state or it is not cultivated yet!", true);
        }
    }

    IEnumerator DestroyCrops()
    {
        CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>();

        //Debug.Log("DestroyCrops ---> cropsPerLand:: " + cropsPerLand + "    isCultivated:: " + _isCultivated + "    canUseLand:: " + _canUseLand);

        //As per this condition, if we want to destroy the crops on the land, there should be crops cultivated...
        if(_isCultivated == true /*&& _canUseland == false*/ && cropsPerLand != null) //_canUseLand is unwanted here as only cultivating action requires _canUseLand condition to be satisfied.
        {
            yield return new WaitForSeconds(0f);

            _isCultivated = false;

            _canUseLand = true;

            _cultivatedCrop = null;

            cropsPerLand.SetDestroyTriggerByPlayer(true);

            TipManager.Instance.ConstructATip("Land crops destroyed!", true);
        }
        else
        {
            // Debug.Log("Command didn't process. Crops Cannot Be Destroyed!!!");
            TipManager.Instance.ConstructATip("There are no crops cultivated to destroy!", true);
        }
    }

    IEnumerator RestoreLand() // will restore the land from monocropping and restore back to its state.
    {
        //CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>(); //Removed this because cropsPerLand will be removed when we destroy crops.

        if (/*cropsPerLand.transform.childCount == 0*/ _isCultivated == false && _isSpreadingFromThis == true && _landFarmingPattern == LandFarmingPatterns.Monocropping)
        {
            yield return new WaitForSeconds(0f);

            if (_landFarmingCropsRecord.Count > 0) { _landFarmingCropsRecord.RemoveAt(_landFarmingCropsRecord.Count - 1); }

            _isSpreadingFromThis = false;

            foreach (var v in _surroundingLandObjects)
            {
                if (v != null && v.tag == "Land")
                {
                    LandManager l = v.GetComponent<LandManager>();

                    if (l != this)
                    {
                        l.SetHasSpreadFromOthers(false);
                        yield return new WaitForSeconds(1f);
                    }
                }
            }

            SwitchLandStatus(/*LandState.Muddy*/_landState);
        }
        else
        {
            //Debug.Log("Command didn't process. Land Cannot Be Restored!!!");
            TipManager.Instance.ConstructATip("Land Cannot Be Restored! Either the land is still cultivated(if so destroy it) or the land is not a monocrop pattern land or it is not affecting any nearby lands!", true);
        }
    }

    #endregion

    #region AI Land Interaction

    public void AIInteraction(string _action = "", string _whoInteracted = "", string _cultivatedCrop = "", SeedItem _aILandCultivatedCrop = null)
    {
        if(this._landOwnership == LandOwnership.AI)
        {
            if (!_playerController.GetWarnedStatusOfPlayer())
            {
                if (TimeManager.Instance.GetCurrentPartOfTheDay() != "Night" && TimeManager.Instance.GetCurrentPartOfTheDay() != "MidNight")
                {
                    AIInteract(_action, _whoInteracted, _cultivatedCrop, _aILandCultivatedCrop);
                }
                else
                {
                    //Debug.Log("Its Either Night Or MidNight. Not The Time For Farming!");
                    //Debug.Log("Only Corp Industry Can Finish Their Current State Work And Go Back! Crop Industry Can't Start New State Work!");

                    if (_whoInteracted == "AI")
                    {
                        AIInteract(_action, _whoInteracted, _cultivatedCrop, _aILandCultivatedCrop);
                    }
                    else
                    {
                        //Debug.Log("Player Can't Do Any Farming Activities!");
                        TipManager.Instance.ConstructATip("Player Can't Do Any Farming Activities! It's Night / MidNight", true);
                    }
                }
            }
            else
            {
                //Debug.Log("Player is warned by Corpporate Industry! Player can't perform actions on their land until the colldown time is reached!");
                TipManager.Instance.ConstructATip("Player is warned by Corpporate Industry! Player can't perform actions on their land until the colldown time is reached!", true);
            }
        }
        else
        {
            //Debug.Log("Wrong Land Interaction! You are in AI Land Interaction!");
        }
    }

    private void AIInteract(string a, string _whoInteracted, string _cultivatedCrop, SeedItem _aILandCultivatedCrop)
    {
        //Possible interactions on AI land - Til, Water, Cultivate, Destroy, Restore

        //Debug.Log("Button Click in " + this + " " + a);

        //TILL
        if (a == PlayerInteractionCommandsForAILand.Till.ToString())
        {
            if (_whoInteracted == "AI")
            {
                StartCoroutine(AITillLand());
            }
            else if (_whoInteracted == "Player")
            {
                StartCoroutine(TillLand());
            }
            else
            {
                Debug.Log("Unknown interaction on AI land!");
            }
        }
        //WATER
        else if (a == PlayerInteractionCommandsForAILand.Water.ToString())
        {
            if (_whoInteracted == "AI")
            {
                StartCoroutine(AIWaterLand());
            }
            else if (_whoInteracted == "Player")
            {
                StartCoroutine(WaterLand());
            }
            else
            {
                Debug.Log("Unknown interaction on AI land!");
            }
        }
        //CULTIVATE
        else if (a == PlayerInteractionCommandsForAILand.Cultivate.ToString())
        {
            if(_whoInteracted == "AI")
            {
                StartCoroutine(AICultivateCrops(_cultivatedCrop, _aILandCultivatedCrop));
            }
            else if(_whoInteracted == "Player")
            {
                StartCoroutine(CultivateCrops());
            }
            else
            {
                Debug.Log("Unknown interaction on AI land!");
            }
        }
        //HARVEST
        else if (a == "Harvest") //Since there is no interaction command - Harvest on AI land, we hardcode it.
        {
            StartCoroutine(AIHarvestCrops());
        }
        //DESTROY
        else if (a == PlayerInteractionCommandsForAILand.Destroy.ToString())
        {
            if (_whoInteracted == "AI")
            {
                StartCoroutine(AIDestroyCrops());
            }
            else if (_whoInteracted == "Player")
            {
                StartCoroutine(DestroyCrops());
            }
            else
            {
                Debug.Log("Unknown interaction on AI land!");
            }
        }
        //RESTORE
        else if (a == PlayerInteractionCommandsForAILand.Restore.ToString())
        {
            if (_whoInteracted == "AI")
            {
                StartCoroutine(AIRestoreLand());
            }
            else if (_whoInteracted == "Player")
            {
                StartCoroutine(RestoreLand());
            }
            else
            {
                Debug.Log("Unknown interaction on AI land!");
            }
        }
    }

    IEnumerator AITillLand()
    {
        if (_landState == LandState.Dry)
        {
            yield return new WaitForSeconds(1f);
            SwitchLandStatus(LandState.Muddy);
        }
        else
        {
            //Debug.Log("Command didn't process. Land Cannot Be Tilled!!!");
        }
    }

    IEnumerator AIWaterLand()
    {
        if (_landState == LandState.Muddy)
        {
            yield return new WaitForSeconds(1f);
            SwitchLandStatus(LandState.Farm);
        }
        else
        {
            //Debug.Log("Command didn't process. Land Cannot Be Watered!!!");
        }
    }

    IEnumerator AICultivateCrops(string _cultivatedCrop, SeedItem _aILandCultivatedCrop)
    {
        //Debug.Log("IsCultivated ::" + _isCultivated + ".  CanUseLand ::" + _canUseLand);

        if (_landState == LandState.Farm && _isCultivated == false && _canUseLand == true && _aILandCultivatedCrop != null)
        {
            yield return new WaitForSeconds(1f);

            Instantiate(_cropsPerLand, transform);
            this._aILandCultivatedCrop = _aILandCultivatedCrop;

            yield return new WaitForSeconds(1f); //Need this 3 secs to have the crops instantiated and let the CropBehaviour class run it's Start function before removing quantity...

            if (_landFarmingCropsRecord.Count < _maxHistoryCapacity) //To keep the history to be only upto last five farming...
            {
                _landFarmingCropsRecord.Add(_aILandCultivatedCrop._cropToYield._itemName);
            }
            else if (_landFarmingCropsRecord.Count >= _maxHistoryCapacity) //To keep the history to be only upto last five farming...
            {
                _landFarmingCropsRecord.RemoveAt(0);
                _landFarmingCropsRecord.Add(_aILandCultivatedCrop._cropToYield._itemName);
            }

            _isCultivated = true;
            _canUseLand = false;
            _isHarvested = false;
        }
        else
        {
            //Debug.Log("Command didn't process. Crops Cannot Be Cultivated!!!");
        }
    }

    IEnumerator AIHarvestCrops()
    {
        CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>();

        //Debug.Log("HarvestCrops ---> cropsPerLand:: " + cropsPerLand + "    isCultivated:: " + _isCultivated + "    canUseLand:: " + _canUseLand);

        //As per this condition, if we want to harvest the land should be farm type or muddy type...
        if ((_landState == LandState.Farm || _landState == LandState.Muddy || _landState == LandState.Dry) && (_isCultivated == true /*&& _canUseland == false*/ && cropsPerLand != null))
        {
            //cropsPerLand.TryHarvest();

            //if (cropsPerLand.GetReadyToHarvest())
            if (AITryHarvest(cropsPerLand))
            {
                cropsPerLand.RemoveQuantity(cropsPerLand.GetQuantityOfItems());

                yield return new WaitForSeconds(2f);

                if (cropsPerLand.GetQuantityOfItems() <= 0)
                {
                    _isCultivated = false;
                    _canUseLand = true;
                    _cultivatedCrop = null;

                    cropsPerLand.SetCropsPerLandDestroyTrigger(true);

                    _isHarvested = true;
                }
            }
            else
            {
                //Debug.Log("Not Ready To Harvest!!!");
            }
        }
        else
        {
            //Debug.Log("Check The Land State Before Harvesting!!!");
            //Debug.Log("Command didn't process. Crops Cannot Be Harvested!!!");
        }
    }

    private bool AITryHarvest(CropsPerLand cropsPerLand = null)
    {
        if(cropsPerLand == null) { cropsPerLand = GetComponentInChildren<CropsPerLand>(); }

        if(cropsPerLand == null) { /*Debug.Log("cropsPerLand is Null! Crops are not yet cultivated!");*/ }
        else {
            cropsPerLand.TryHarvest();

            if (cropsPerLand.GetReadyToHarvest())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    IEnumerator AIDestroyCrops()
    {
        CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>();

        //Debug.Log("DestroyCrops ---> cropsPerLand:: " + cropsPerLand + "    isCultivated:: " + _isCultivated + "    canUseLand:: " + _canUseLand);

        //As per this condition, if we want to destroy the crops on the land, there should be crops cultivated...
        if (_isCultivated == true /*&& _canUseland == false*/ && cropsPerLand != null) //_canUseLand is unwanted here as only cultivating action requires _canUseLand condition to be satisfied.
        {
            yield return new WaitForSeconds(1f);

            _isCultivated = false;

            _canUseLand = true;

            _cultivatedCrop = null;

            cropsPerLand.SetDestroyTriggerByPlayer(true);
        }
        else
        {
            //Debug.Log("Command didn't process. Crops Cannot Be Destroyed!!!");
        }
    }

    IEnumerator AIRestoreLand()
    {
        //CropsPerLand cropsPerLand = GetComponentInChildren<CropsPerLand>(); //Removed this because cropsPerLand will be removed when we destroy crops.

        if (/*cropsPerLand.transform.childCount == 0*/ _isCultivated == false && _isSpreadingFromThis == true && _landFarmingPattern == LandFarmingPatterns.Monocropping)
        {
            yield return new WaitForSeconds(1f);

            if (_landFarmingCropsRecord.Count > 0) { _landFarmingCropsRecord.RemoveAt(_landFarmingCropsRecord.Count - 1); }

            _isSpreadingFromThis = false;

            foreach (var v in _surroundingLandObjects)
            {
                if (v != null && v.tag == "Land")
                {
                    LandManager l = v.GetComponent<LandManager>();

                    if (l != this)
                    {
                        l.SetHasSpreadFromOthers(false);
                        yield return new WaitForSeconds(1f);
                    }
                }
            }
            SwitchLandStatus(LandState.Muddy);
        }
        else
        {
            //Debug.Log("Command didn't process. Land Cannot Be Restored!!!");
        }
    }

    #endregion

    #region Land Time Updater

    //TIMER FOR LAND STATE
    //Sets new _ownLandStateTimer for the land state that will be changed over time...
    void LandStateTimeSetter() //Runs only once when the land state is chnaged...
    {
        if(_gameTimer != null)
        {
            _ownLandStateTimer = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);
        }
    }

    //Keeps track of the land state changes over time...
    void LandStateTimeTracker(LandState landState) //Runs every frame (from Update function) when _ownLandStateTimer and _gameTimer is not null...
    {
        switch (landState)
        {
            case LandState.Dry:

                if(_isRaining && _is6HoursNow)
                {
                    _ownLandStateTimer = null;
                    SwitchLandStatus(LandState.Clay);
                }

                break;


            case LandState.Clay: //Takes 48 hours to recover clay land to dry land.

                if (_ownLandStateTimer != null && _gameTimer != null)
                {
                    //Check for a day has passed or not in hours.
                    _landStateHours = TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownLandStateTimer, _gameTimer);

                    //Changes to dry if the previous rainfall is more than 6 hours, else changes to muddy.
                    if(_landStateHours >= 90) //90 minutes in 1 hour and 30 minutes
                    {
                        if (!_isRaining)
                        {
                            _ownLandStateTimer = null;
                            SwitchLandStatus(LandState.Dry);
                        }
                        //When it is more than 8 hours since land changed to clay and when it stops raining, it shouldn't change immediately to dry.
                        else if (_isRaining && _is6HoursNow)
                        {
                            _ownLandStateTimer = null;
                            SwitchLandStatus(LandState.Clay);
                        }
                    }
                }
                else
                {
                    //Debug.LogError("Error in LandManager :: _ownLandStateTimer && _gameTimer");
                }

                break;


            case LandState.Muddy: //Takes 24 hours to change from muddy land to dry land.

                if (_ownLandStateTimer != null && _gameTimer != null)
                {
                    //Check for a day has passed or not in hours.
                    _landStateHours = TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownLandStateTimer, _gameTimer);

                    if (_isRaining && _is6HoursNow)
                    {
                        _ownLandStateTimer = null;
                        SwitchLandStatus(LandState.Clay);
                    }
                    else if (_landStateHours >= 360) //360 minutes is 6 hour.
                    {
                        _ownLandStateTimer = null;
                        SwitchLandStatus(LandState.Dry);
                    }
                }
                else
                {
                    //Debug.LogError("Error in LandManager :: _ownLandStateTimer && _gameTimer");
                }

                break;


            case LandState.Farm:
                
                if (_ownLandStateTimer != null && _gameTimer != null)
                {
                    //Check for a day has passed or not in hours.
                    _landStateHours = TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownLandStateTimer, _gameTimer);

                    if (_isRaining && _is6HoursNow)
                    {
                        _ownLandStateTimer = null;
                        SwitchLandStatus(LandState.Clay);
                    }
                    else if (_landStateHours >= 360) //360 minutes is 6 hours
                    {
                        _ownLandStateTimer = null;
                        SwitchLandStatus(LandState.Muddy);
                    }
                }
                else
                {
                    //Debug.LogError("Error in LandManager :: _ownLandStateTimer && _gameTimer");
                }

                break;


            case LandState.Grassy:

                //Nothing...

                break;


            default:

                //Nothing...

                break;
        }
    }

    //TIMER FOR LAND FARMING PATTERN
    //Sets new _ownLandPatternTimer for the land farming pattern that will be changed over time...
    void LandFarmingPatternTimeSetter()
    {
        if (_gameTimer != null)
        {
            _ownLandPatternTimer = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);
        }
    }

    //Keeps track of the land farming pattern changes over time...
    void LandFarmingPatternTimeTracker(LandFarmingPatterns landFarmingPattern)
    {
        switch (landFarmingPattern)
        {
            case LandFarmingPatterns.NoPattern:

                //Nothing

                break;

            case LandFarmingPatterns.PotentialMonocropping:

                if (_ownLandPatternTimer != null && _gameTimer != null)
                {
                    _landFarmingPatternHours = TimeManager.Instance.DifferenceInTimeUsingHours(_ownLandPatternTimer, _gameTimer);

                    if(_landFarmingPatternHours >= 1f)
                    {
                        //Nothing happens in potential monocropping!
                    }
                }

                break;

            case LandFarmingPatterns.Monocropping:

                if (_ownLandPatternTimer != null && _gameTimer != null)
                {
                    _landFarmingPatternHours = TimeManager.Instance.DifferenceInTimeUsingHours(_ownLandPatternTimer, _gameTimer);

                    //Will start spreading in 1 hour after changing to monocropping pattern.
                    if (_landFarmingPatternHours >= 1f && !_isSpreadingFromThis)
                    {
                        _isSpreadingFromThis = true;
                        StartCoroutine(SpreadLandPattern(landFarmingPattern));
                        TipManager.Instance.ConstructATip("One or more of your lands has started spreading monocropping! Find, Destroy(if cultivated) & Restore them!", true);
                    }
                }

                break;

            case LandFarmingPatterns.CropRotation:

                if (_ownLandPatternTimer != null && _gameTimer != null)
                {
                    //Nothing happens in crop rotation!
                }

                break;

            default:

                SwitchLandPattern(LandFarmingPatterns.NoPattern);

                break;
        }
    }

    #endregion

    #region Land Pattern - Land Health - Land Nutrition Level Updater & Spreader

    //Function to call all the Pattern and Health related functions...
    void UpdateLandPatternLandHealthAndLandNutritionLevel()
    {
        LandFarmingPatternDetector();
        EvaluateLandStateHealth();
        EvaluateLandFarmingPatternHealth();
        EvaluateLandNutritionLevel();
        EvaluateOverallLandHealth();
        this.transform.parent.GetComponent<PlotManager>().UpdatePlotPatternAndHealth();
    }

    #region Land Pattern Updater

    //Land Farming Pattern Detector... Called everytime when a land is cultiavted...
    void LandFarmingPatternDetector()
    {
        if (!_hasSpreadedFromOthers && !_isSpreadingFromThis) //Enters the conditions only when the land patterns are not spreaded from others...
        {
            if (_landFarmingCropsRecord.Count == 2) //Enters only when count is atleast 2...
            {
                //When the count is equal to 2, checking the last 2 crops cultivated to evaluate the pattern...
                if (_landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] == _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 2])
                {
                    SwitchLandPattern(LandFarmingPatterns.PotentialMonocropping);
                }
            }
            else
            {
                if (_landFarmingCropsRecord.Count >= 3) //Enters only when count is atleast 3...
                {
                    //Checking the last 3 crops cultivated to evaluate the pattern...
                    if (_landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] == _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 2] &&
                        _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] == _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 3])
                    //If all the last 3 crops are same, it results in a monocropping practice...
                    {
                        SwitchLandPattern(LandFarmingPatterns.Monocropping);
                    }
                    else if (_landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] == _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 3] &&
                        _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] != _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 2])
                    {
                        //If the last 3 crops are of alternate type, it results in a crop rotation practice...
                        SwitchLandPattern(LandFarmingPatterns.CropRotation);
                    }
                    else if (_landFarmingCropsRecord[_landFarmingCropsRecord.Count - 1] == _landFarmingCropsRecord[_landFarmingCropsRecord.Count - 2])
                    {
                        //If the last 2 crops are of same type, it results in potential monocropping practice...
                        SwitchLandPattern(LandFarmingPatterns.PotentialMonocropping);
                    }
                }
                else
                {
                    SwitchLandPattern(LandFarmingPatterns.NoPattern);
                }
            }
        }
        else
        {
            //Debug.Log("Spreading!... Patterns won't be evaluated using cropping history method until spreading is controlled!...");
            //TipManager.Instance.ConstructATip("Patterns won't be evaluated using cropping history method until spreading is controlled!", true);
        }
    }

    #endregion

    #region Land Health Updater

    //Health for Land State Health...
    void EvaluateLandStateHealth()
    {
        if (_landState == LandState.Dry)
        {
            if (_landStateHealth > 30.0f) { _landStateHealth -= 0.5f; }
            else if (_landStateHealth < 30.0f) { _landStateHealth += 0.5f; }
        }
        else if(_landState == LandState.Grassy)
        {
            if (_landStateHealth > 20.0f) { _landStateHealth -= 0.5f; }
            else if (_landStateHealth < 20.0f) { _landStateHealth += 0.5f; }
        }
        else if(_landState == LandState.Clay)
        {
            if (_landStateHealth > 10.0f) { _landStateHealth -= 0.5f; }
            else if (_landStateHealth < 10.0f) { _landStateHealth += 0.5f; }
        }
        else if (_landState == LandState.Muddy)
        {
            if (_landStateHealth > 50.0f) { _landStateHealth -= 0.5f; }
            else if (_landStateHealth < 50.0f) { _landStateHealth += 0.5f; }
        }
        else if (_landState == LandState.Farm)
        {
            if (_landStateHealth > 100.0f) { _landStateHealth -= 0.5f; }
            else if (_landStateHealth < 100.0f) { _landStateHealth += 0.5f; }
        }
    }

    //Health for Land Farming Pattern Health...
    void EvaluateLandFarmingPatternHealth()
    {
        if(_landFarmingPattern == LandFarmingPatterns.NoPattern) //NO PATTERN
        {
            //LAND FARMING PATTERN HEALTH
            if (_landFarmingPatternHealth > 100.0f) { _landFarmingPatternHealth -= 0.5f; }
            else if (_landFarmingPatternHealth < 100.0f) { _landFarmingPatternHealth += 0.5f; }
        }
        else if(_landFarmingPattern == LandFarmingPatterns.CropRotation) //CROP ROTATION
        {
            //LAND FARMING PATTERN HEALTH
            if(_landFarmingPatternHealth > 100.0f) { _landFarmingPatternHealth -= 0.5f; }
            else if(_landFarmingPatternHealth < 100.0f) { _landFarmingPatternHealth += 0.5f; }
        }
        else if(_landFarmingPattern == LandFarmingPatterns.PotentialMonocropping) //POTENTIAL MONOCROPPING
        {
            //LAND FARMING PATTERN HEALTH
            if (_landFarmingPatternHealth > 50.0f) { _landFarmingPatternHealth -= 0.5f; }
            else if(_landFarmingPatternHealth < 50.0f) { _landFarmingPatternHealth += 0.5f; }
        }
        else if(_landFarmingPattern == LandFarmingPatterns.Monocropping) //MONOCROPPING
        {
            //LAND FARMING PATTERN HEALTH
            if (_landFarmingPatternHealth > 0.0f) { _landFarmingPatternHealth -= 0.5f; }
            else if(_landFarmingPatternHealth < 0.0f) { _landFarmingPatternHealth += 0.5f; }
        }
    }

    //Evaluate Overall Health...
    void EvaluateOverallLandHealth()
    {
        _overallLandHealth = (_landStateHealth + _landFarmingPatternHealth) / 2;
    }

    #endregion

    #region Land Nutrition Level Updater

    //Evaluate Nutrition Level of land...
    void EvaluateLandNutritionLevel()
    {
        if (_landFarmingPattern == LandFarmingPatterns.NoPattern) //NO PATTERN
        {
            //LAND NUTRITION LEVEL
            if (_landNutritionLevel > 100.0f) { _landNutritionLevel -= 0.5f; }
            else if (_landNutritionLevel < 100.0f) { _landNutritionLevel += 0.5f; }
        }
        else if (_landFarmingPattern == LandFarmingPatterns.CropRotation) //CROP ROTATION
        {
            //LAND NUTRITION LEVEL
            if (_landNutritionLevel > 100.0f) { _landNutritionLevel -= 0.5f; }
            else if (_landNutritionLevel < 100.0f) { _landNutritionLevel += 0.5f; }
        }
        else if (_landFarmingPattern == LandFarmingPatterns.PotentialMonocropping) //POTENTIAL MONOCROPPING
        {
            //LAND NUTRITION LEVEL
            if (_landNutritionLevel > 50.0f) { _landNutritionLevel -= 0.5f; }
            else if (_landNutritionLevel < 50.0f) { _landNutritionLevel += 0.5f; }
        }
        else if (_landFarmingPattern == LandFarmingPatterns.Monocropping) //MONOCROPPING
        {
            //LAND NUTRITION LEVEL
            if (_landNutritionLevel > 0.0f) { _landNutritionLevel -= 0.5f; }
            else if (_landNutritionLevel < 0.0f) { _landNutritionLevel += 0.5f; }
        }
    }

    #endregion

    #region Land Pattern Spreader

    //A call to this function starts the Monocropping spreading process...
    IEnumerator SpreadLandPattern(LandFarmingPatterns landFarmingPattern)
    {
        foreach (var v in _surroundingLandObjects)
        {
            if (v != null && v.tag == "Land")
            {
                LandManager l = v.GetComponent<LandManager>();

                if (l != this && !l.GetIsSpreadingFromThis() && !l.GetHasSpreadedFromOthers())
                {
                    l.SetHasSpreadFromOthers(true);
                    l.SwitchLandPattern(landFarmingPattern);
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }

    #endregion

    #endregion

    #region Finding Nearest Objects
    //Function to get the nearest objects data... (Since the land is a static object in the scene, it can be called only once to get the surrounding lands)
    void FindSurroundingLands()
    {
        //https://docs.unity3d.com/ScriptReference/Physics.OverlapSphereNonAlloc.html
        Physics.OverlapSphereNonAlloc(this.gameObject.transform.position, 5.0f, _surroundingLandObjects);
    }
    #endregion

    #region Observer Design Pattern Functions
    public void InteractInfoUpdate(RaycastHit outHit)
    {
        if (outHit.collider.tag == "Land" && outHit.collider.transform == this.transform && outHit.collider.GetComponent<LandManager>()._landOwnership == LandOwnership.Player) //.transform.GetComponent<LandManager>()
        {
            //Debug.Log("outHit entered in LandManager!");
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfFarmingCommandsForPlayerLand);
            //Debug.Log("outHit Data loaded from LandManager!");
        }
        else if (outHit.collider.transform == this.transform && outHit.collider.GetComponent<LandManager>()._landOwnership == LandOwnership.AI)
        {
            //Debug.Log("outHit entered in LandManager!");
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfFarmingCommandsForAILand);
            //Debug.Log("outHit Data loaded from LandManager!");
        }
        else if (outHit.collider.transform == null)
        {
            //Debug.Log("outHit entered in LandManager!");
            //Debug.LogError("_outHit is null in LandManager");
        }
    }

    public void TickUpdate(GameTimer gameTimer)
    {
        if (_gameTimer == null) { _gameTimer = gameTimer; }
    }

    public void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData)
    {
        if(equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            this._equipedItemSlotDataCS = new GameItemsSlotData(equipedItemSlotDataCS);
        }
        else
        {
            this._equipedItemSlotDataCS.EmptyIt(true);
        }

        if (equipedItemSlotDataTE.GetGameItemsData() != null)
        {
            this._equipedItemSlotDataTE = new GameItemsSlotData(equipedItemSlotDataTE);
        }
        else
        {
            this._equipedItemSlotDataTE.EmptyIt(true);
        }
    }

    public void RainInfo(bool isRaining, bool hasBeenFor6Hours)
    {
        _isRaining = isRaining;
        _is6HoursNow = hasBeenFor6Hours;
    }
    #endregion
}