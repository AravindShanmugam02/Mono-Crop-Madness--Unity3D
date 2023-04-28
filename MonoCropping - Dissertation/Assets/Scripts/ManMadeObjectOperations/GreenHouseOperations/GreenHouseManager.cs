using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GreenHouseManager : MonoBehaviour, InventoryInfo, iInteractInfo
{
    public GreenHouseOwnership GetGreenHouseOwnership() { return _greenHouseOwnership; }

    //Enum representing Green House Ownership...
    public enum GreenHouseOwnership
    {
        Player, AI
    }
    [SerializeField] private GreenHouseOwnership _greenHouseOwnership;

    //Enum representing PlantBoxes in the Green House...
    public enum PlantBox
    {
        One, Two, Three, Four
    }
    //[SerializeField] private PlantBox _plantBox;

    //Enum representing Player Interaction Commands For Green House...
    public enum PlayerInteractionCommandsForGreenHouse
    {
        SowSeeds, HarvestCrops, RefillWateringSystem, ViewInsideGreenhouse
    }
    //[SerializeField] private PlayerInteractionCommandsForGreenHouse _greenHouseCommand;

    //Dictionaries to handle PlantBox Information...
    [SerializeField] private Dictionary<PlantBox, SeedItem> _plantBoxWithSeedItemDictionary;
    [SerializeField] private Dictionary<GreenHousePlantBoxManager, float> _plantBoxManagerWithGrowthProgressDictionary;
    [SerializeField] private Dictionary<PlantBox, GreenHousePlantBoxManager> _plantBoxWithPlantBoxManager;

    //To keep track of water level...
    [SerializeField] private int _waterSystemLevel;

    //No of seeds required per cultivation per plant box...
    [SerializeField] private int _reqNumOfSeedsPerPlantBoxPerCultivation;

    //To store the equipped item information...
    [SerializeField] private GameItemsSlotData _equipedItemSlotDataCS;
    [SerializeField] private GameItemsSlotData _equipedItemSlotDataTE;

    //To handle water system of thr green house...
    private GreenHouseWaterSystemManager _gHWaterSystemManager;

    //To handle open and close HUD with parent help...
    private GreenHouseBuildingManager _greenHouseBuildingManager;

    //To get the plantboxes of this green house...
    private GameObject _plantBoxContainer;

    //Handles for HUD display...
    //To handle Plant Box HUD Container...
    private GameObject _gHHUDPlantBoxContainer;
    //List of PlantBoxHUD in a Greenhouse...
    private List<Image> _gHHUDPlantBoxlist;
    //Handle for Green house Water System HUD...
    private GameObject _gHHUDWaterSystem;

    //To store the green house commands in a list...
    private List<string> _listOfGreenHouseCommandsForPlayer;

    ////To check if greenhouse is being viewed or not...
    //private bool _isGreenhouseOpen;
    //private bool _isGreenhouseviewed;

    //Default Slot sprite...
    [SerializeField] private Sprite _defaultSpriteToDisplay;

    //Bool to identify which greenhouse is active.
    [SerializeField] private bool _isThisGreenhouseViewed;

    // Start is called before the first frame update
    void Start()
    {
        //Always remember enums can't be inside start. Cuz enums are like class or struct, but this start() is a method.
        //So, a class(or like type) can't be declared isnide a method.

        _plantBoxWithSeedItemDictionary = new Dictionary<PlantBox, SeedItem>();
        _plantBoxManagerWithGrowthProgressDictionary = new Dictionary<GreenHousePlantBoxManager, float>();
        _plantBoxWithPlantBoxManager = new Dictionary<PlantBox, GreenHousePlantBoxManager>();
        _gHHUDPlantBoxlist = new List<Image>();

        _listOfGreenHouseCommandsForPlayer = new List<string>();
        //https://stackoverflow.com/a/14971637 - //Can also use using System; in the top instead of System.Enum below.
        _listOfGreenHouseCommandsForPlayer = System.Enum.GetNames(typeof(PlayerInteractionCommandsForGreenHouse)).ToList(); //using System.Linq for .ToList().

        //Debug.Log("List Of Green House Commands :: " + _listOfGreenHouseCommandsForPlayer.Count);

        //_defaultSpriteToDisplay = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/In-Game/EmptySlot.png", typeof(Sprite)) as Sprite;

        _greenHouseBuildingManager = transform.GetComponentInParent<GreenHouseBuildingManager>(); //Gets this component from any of its parents...
                                                                                                  //Hierarchy won't be an issue even if the object that has this component
                                                                                                  //isn't its direct parent.
        _gHWaterSystemManager = transform.Find("GreenHouseWaterSystem").GetComponent<GreenHouseWaterSystemManager>();
        _plantBoxContainer = GameObject.Find("GreenHousePlantBoxContainer");

        if (_gHWaterSystemManager == null)
        {
            Debug.LogError("Check Green House Manager - _gHWaterSystemManager is null!");
        }
        if (_plantBoxContainer == null)
        {
            Debug.LogError("Check Green House Manager - _plantBoxContainer is null!");
        }
        if (_greenHouseBuildingManager == null)
        {
            Debug.LogError("Check Green House Manager - _greenHouseBuildingManager is null!");
        }

        //Getting these from parent object because this is a prefab and when this is instantiated,
        //we won't know if these objects will be active at that moment.
        _gHHUDPlantBoxContainer = _greenHouseBuildingManager.GetHUDPlantBoxContainer();
        _gHHUDWaterSystem = _greenHouseBuildingManager.GetHUDWaterSystem();

        if (_gHHUDPlantBoxContainer == null)
        {
            Debug.LogError("Check Green House Manager - _gHHUDPlantBoxContainer is null!");
        }
        if (_gHHUDWaterSystem == null)
        {
            Debug.LogError("Check Green House Manager - _gHHUDWaterSystem is null!");
        }

        //https://stackoverflow.com/a/856165 //var myEnumMemberCount = Enum.GetNames(typeof(MyEnum)).Length;
        for (int i = 0; i < System.Enum.GetNames(typeof(PlantBox)).Length; i++)
        {
            _plantBoxWithSeedItemDictionary.Add((PlantBox)i, null);
        }

        for (int i = 0; i < _plantBoxContainer.transform.childCount; i++)
        {
            _plantBoxWithPlantBoxManager.Add((PlantBox)i, _plantBoxContainer.transform.GetChild(i).GetComponent<GreenHousePlantBoxManager>());
            _plantBoxManagerWithGrowthProgressDictionary.Add(_plantBoxContainer.transform.GetChild(i).GetComponent<GreenHousePlantBoxManager>(), 0f);
        }


        //This is because we can't directly assgin a list to another one.
        List<Image> temp = _greenHouseBuildingManager.GetHUDPlantBoxList();
        foreach(var item in temp)
        {
            _gHHUDPlantBoxlist.Add(item);
        }

        _equipedItemSlotDataCS = new GameItemsSlotData();
        _equipedItemSlotDataTE = new GameItemsSlotData();

        _waterSystemLevel = 0;
        //_isGreenhouseOpen = false;
        //_isGreenhouseviewed = false;
        _reqNumOfSeedsPerPlantBoxPerCultivation = 3;

        InventoryDisplayManager.Instance.AddAsInventoryInfoListener(this);
        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);

        if (_plantBoxWithSeedItemDictionary == null && _plantBoxWithSeedItemDictionary.Count == 0)
        {
            Debug.LogError("Check Green House Manager - _plantBoxWithSeedItemDictionary is null/empty!");
        }

        if (_plantBoxWithPlantBoxManager == null && _plantBoxWithPlantBoxManager.Count == 0)
        {
            Debug.LogError("Check Green House Manager - _plantBoxWithPlantBoxManager is null/empty!");
        }

        if (_plantBoxManagerWithGrowthProgressDictionary == null && _plantBoxManagerWithGrowthProgressDictionary.Count == 0)
        {
            Debug.LogError("Check Green House Manager - _plantBoxManagerWithGrowthProgressDictionary is null/empty!");
        }

        if (_gHHUDPlantBoxlist == null && _gHHUDPlantBoxlist.Count == 0)
        {
            Debug.LogError("Check Green House Manager - _gHHUDPlantBoxlist is null/empty!");
        }

        //_viewInsideGreenhouse.SetActive(false);
        //_backButton.gameObject.SetActive(false);

        _isThisGreenhouseViewed = false;
    }

    // Update is called once per frame
    void Update()
    {
        _waterSystemLevel = _gHWaterSystemManager.GetCurrentWaterLevel();

        if (_greenHouseBuildingManager.GetIsGreenhouseViewed() && _isThisGreenhouseViewed)
        {
            RenderGreenhouseView();
        }
        else if (!_greenHouseBuildingManager.GetIsGreenhouseViewed() && _isThisGreenhouseViewed)
        {
            _isThisGreenhouseViewed = false;
        }
    }

    public void PlayerInteraction(string action = "")
    {
        if (this._greenHouseOwnership == GreenHouseOwnership.Player)
        {
            PlayerInteract(action);
        }
        else
        {
            //Debug.Log("Wrong Interaction! You are not interacting with Player Greenhouse!");
            TipManager.Instance.ConstructATip("Wrong Interaction! You are not interacting with Player Greenhouse!", true);
        }
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        if (action == PlayerInteractionCommandsForGreenHouse.SowSeeds.ToString())
        {
            StartCoroutine(SowSeeds());
        }
        else if (action == PlayerInteractionCommandsForGreenHouse.HarvestCrops.ToString())
        {
            StartCoroutine(HarvestCrops());
        }
        else if (action == PlayerInteractionCommandsForGreenHouse.RefillWateringSystem.ToString())
        {
            StartCoroutine(RefillWateringSystem());
        }
        else if(action == PlayerInteractionCommandsForGreenHouse.ViewInsideGreenhouse.ToString())
        {
            _isThisGreenhouseViewed = true;
            ViewInsideGreenhouse();
        }
    }

    IEnumerator SowSeeds()
    {
        bool isSowingSeedsSuccessfull = false;

        if (_equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            if (_equipedItemSlotDataCS.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed && _equipedItemSlotDataCS.GetQuantity() >= _reqNumOfSeedsPerPlantBoxPerCultivation)
            {
                SeedItem seedsToSow = _equipedItemSlotDataCS.GetGameItemsData() as SeedItem;

                if (_gHWaterSystemManager.CheckIfWaterLevelIsSufficient(seedsToSow._reqWaterToGrow))
                {
                    foreach (var item in _plantBoxWithPlantBoxManager)
                    {
                        if (!item.Value.GetIsPlantBoxCultivated())
                        {
                            if (!item.Value.CheckTheLastPlantedCropIsSame(seedsToSow))
                            {
                                item.Value.PlantInThisBox(seedsToSow);

                                //https://docs.unity3d.com/ScriptReference/WaitUntil.html
                                //https://forum.unity.com/threads/setting-system-function-bool.623533/#post-4176481
                                yield return new WaitUntil(() => item.Value.GetIsPlantBoxCultivated());

                                foreach(var item1 in _plantBoxWithSeedItemDictionary)
                                {
                                    if(item1.Key == item.Key)
                                    {
                                        _plantBoxWithSeedItemDictionary[item1.Key] = seedsToSow; //https://stackoverflow.com/a/1243724
                                        break;
                                    }
                                }

                                _gHWaterSystemManager.MinusWaterFromWaterSystem(seedsToSow._reqWaterToGrow);

                                InventoryManager.Instance.RemoveQuantityFromEquiped(InventorySlot.InventorySlotType.CS, _reqNumOfSeedsPerPlantBoxPerCultivation); //Removig quantity from Inventory...

                                isSowingSeedsSuccessfull = true;
                                break;
                            }
                            else
                            {
                                //Debug.Log("Plant Box " + item.Key.ToString() + " cannot be planted as the last cultivated crop is same as the one you're trying to cultivate!");
                                TipManager.Instance.ConstructATip("Plant Box " + item.Key.ToString() + " cannot be planted as the last cultivated crop is same as the one you're trying to cultivate!", true);
                            }
                        }
                        else
                        {
                            //Debug.Log("Plant Box "+item.Key.ToString()+" cannot be planted as it is already cultivated!");
                            TipManager.Instance.ConstructATip("Plant Box " + item.Key.ToString() + " cannot be planted as it is already cultivated!", true);
                        }
                    }

                    if (isSowingSeedsSuccessfull)
                    {
                        //Debug.Log("Plant Box cultivated successfully!");
                        TipManager.Instance.ConstructATip("Plant Box cultivated successfully!", true);
                    }
                    else
                    {
                        //Debug.Log("Plant Box cultivation failed!");
                        TipManager.Instance.ConstructATip("Plant Box cultivation failed!", true);
                    }
                }
                else
                {
                    //Debug.Log("There isn't sufficient level of water for a new cultivation! Refil the watering system!");
                    TipManager.Instance.ConstructATip("There isn't sufficient level of water for a new cultivation! Refil the watering system!", true);
                }
            }
            else
            {
                //Debug.Log("The equiped item is either not a seed or is a seed with low quantity!");
                TipManager.Instance.ConstructATip("The equiped item is either not a seed or is a seed with low quantity!", true);
            }
        }
        else
        {
            //Debug.Log("You have not equiped anything. Equip a seed and try again!");
            TipManager.Instance.ConstructATip("You have not equiped anything. Equip a seed and try again!", true);
        }

        isSowingSeedsSuccessfull = false;
    }

    IEnumerator HarvestCrops()
    {
        bool isHarvestCropsSuccessfull = false;

        foreach(var item in _plantBoxWithPlantBoxManager)
        {
            if (item.Value.GetIsPlantBoxCultivated() && item.Value.GetIsPlantBoxReadyForHarvest() && !item.Value.GetIsPlantBoxGrowing())
            {
                //We get the CropItem from the SeedItem and store it as GameItemData. This is because when trying to add to inventory it required a GameItemsData which is a crop not seed.
                GameItemsData gameItemsCropData = item.Value.GetCurrentCultivatedPlant()._cropToYield;

                //AddObjectsToInventory() returns the number of yield items that is remaining in the quantity to harvest value that was sent to add in inventory...
                int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(gameItemsCropData, item.Value.GetQuantityOfItemsToHarvest());
                
                //This is to check if the inventory is full. if the remaining quantity is equal to quantity to harvest, then nothing is added to the inventory.
                if(remainingQuantity >= item.Value.GetQuantityOfItemsToHarvest())
                {
                    //Debug.Log("Harvest not successfull! Inventory might be full with no empty spaces! Trying next item if it can be stacked atleast!");
                    TipManager.Instance.ConstructATip("Harvest not successfull! Inventory might be full with no empty spaces! Trying next item if it can be stacked atleast!", true);
                    continue;
                }
                
                //After adding the ready to harvest items into inventory, the remaining quantity is updated in the plantbox...
                item.Value.HarvestThisBox(item.Value.GetQuantityOfItemsToHarvest() - remainingQuantity);
                TipManager.Instance.ConstructATip("Harvest successfull!", true);
                isHarvestCropsSuccessfull = true;
            }
            else
            {
                //Debug.Log("Either the Plant Box is not cultivated or the planted crop is not ready to harvest!");
                TipManager.Instance.ConstructATip("Either the Plant Box is not cultivated or the planted crop is not ready to harvest!", true);
            }

            if (isHarvestCropsSuccessfull)
            {
                if(item.Value.GetCurrentCultivatedPlant() == null)
                {
                    //Update this dictionary after harvest... it contains SeedItem info.
                    _plantBoxWithSeedItemDictionary[item.Key] = null;
                }

                break;
            }
        }

        if (!isHarvestCropsSuccessfull)
        {
            //Debug.Log("None of the plant boxes have been harvested!");
            TipManager.Instance.ConstructATip("None of the plant boxes have been harvested!", true);
        }

        isHarvestCropsSuccessfull = false;
        yield return null;
    }

    IEnumerator RefillWateringSystem()
    {
        if (_equipedItemSlotDataTE.GetGameItemsData() != null)
        {
            if (_equipedItemSlotDataTE.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool && _equipedItemSlotDataTE.GetGameItemsData()._itemName == "Watering Can")
            {
                GameItemsSlotData wateringCanTool = new GameItemsSlotData(_equipedItemSlotDataTE.GetGameItemsData(), _equipedItemSlotDataTE.GetQuantity());

                if(wateringCanTool.GetQuantity() > 0)
                {
                    int _quantityToRefill = (int)_gHWaterSystemManager.CheckHowMuchWaterLevelCanBeRefilled();

                    if(_quantityToRefill <= wateringCanTool.GetQuantity()) //This check won't let Watering Can tool quantity to go in negative values.
                    {
                        InventoryManager.Instance.RemoveQuantityFromEquiped(InventorySlot.InventorySlotType.TE, _quantityToRefill);

                        _gHWaterSystemManager.AddWaterToWaterSystem(_quantityToRefill);
                    }
                    else if(_quantityToRefill > wateringCanTool.GetQuantity()) //This check won't let Watering Can tool quantity to go in negative values.
                    {
                        InventoryManager.Instance.RemoveQuantityFromEquiped(InventorySlot.InventorySlotType.TE, wateringCanTool.GetQuantity());

                        _gHWaterSystemManager.AddWaterToWaterSystem(wateringCanTool.GetQuantity());
                    }
                    else
                    {
                        //Debug.Log("This place in if-else tree shouldn't be reached as the above conditions satisfy every scenario!");
                    }
                }
                else
                {
                    //Debug.Log("Watering can is empty! Refil the watering can!");
                    TipManager.Instance.ConstructATip("Watering can is empty! Refil the watering can!", true);
                }
            }
            else
            {
                //Debug.Log("The equiped item is either not a tool or is a tool which is not used for watering!");
                TipManager.Instance.ConstructATip("The equiped item is either not a tool or is a tool which is not used for watering!", true);
            }
        }
        else
        {
            //Debug.Log("You have not equiped anything. Equip a tool and try again!");
            TipManager.Instance.ConstructATip("You have not equiped anything. Equip a tool and try again!", true);
        }
        
        yield return null;
    }

    private void ViewInsideGreenhouse()
    {
        _greenHouseBuildingManager.SetIsGreenhouseOpen(true);
    }

    private void RenderGreenhouseView()
    {
        for (int i = 0; i < _gHHUDPlantBoxlist.Count; i++)
        {
            TextMeshProUGUI headingText = _gHHUDPlantBoxlist[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            Image itemImage = _gHHUDPlantBoxlist[i].transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();

            TextMeshProUGUI plantNameText = _gHHUDPlantBoxlist[i].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI harvestStatusText = _gHHUDPlantBoxlist[i].transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI quantityText = _gHHUDPlantBoxlist[i].transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();

            if (headingText == null || itemImage == null || plantNameText == null || harvestStatusText == null 
                || quantityText == null)
            {
                Debug.LogError("Error in RenderGreenhouseView method in GreenHouse Manager");
                Debug.LogError("Check headingText, itemImage, plantNameText, harvestStatusText, " +
                    "quantityText for errors in RenderGreenhouseView method in GreenHouse Manager");
            }

            headingText.text = "Plant Box " + ((PlantBox)i).ToString();

            if (_plantBoxWithSeedItemDictionary[key:((PlantBox)i)] != null)
            {
                itemImage.sprite = _plantBoxWithSeedItemDictionary[key: ((PlantBox)i)]._thumbnail;
                plantNameText.text = _plantBoxWithSeedItemDictionary[key: ((PlantBox)i)]._cropToYield._itemName;
                if (_plantBoxWithPlantBoxManager[key: ((PlantBox)i)].GetIsPlantBoxReadyForHarvest())
                {
                    harvestStatusText.text = "Ready";
                }
                else
                {
                    harvestStatusText.text = "Not Ready";
                }
                quantityText.text = _plantBoxWithPlantBoxManager[key: ((PlantBox)i)].GetQuantityOfItemsToHarvest().ToString();
            }
            else
            {
                itemImage.sprite = _defaultSpriteToDisplay;
                plantNameText.text = "";
                harvestStatusText.text = "";
                quantityText.text = "";
            }
        }

        Slider waterLevelSlider = _gHHUDWaterSystem.transform.GetChild(1).GetComponent<Slider>();
        TextMeshProUGUI waterLevelText = _gHHUDWaterSystem.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        waterLevelSlider.value = _waterSystemLevel;
        waterLevelText.text = waterLevelSlider.value.ToString() + "/" + waterLevelSlider.maxValue;
    }

    public void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData)
    {
        if (equipedItemSlotDataCS.GetGameItemsData() != null)
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

    public void InteractInfoUpdate(RaycastHit outHit)
    {
        #region COMMENTS ON WHY HITOBJ.COLLIDER IS ROUTING TO ITS PARENT
        //Since there is a special case with Green House object, the outHit.collider will not be the GreenHouse object itself but its children.
        //Its direct children have tag "GreenHouse". That is the reason why ray hits and detects the GreenHouse.
        //Due to which the below condition would fail all the time if we treat outHit as the actual GreenHouse object.
        //It is because of the fact that the GreenHouse object doesn't have any mesh or collider of its own. Its children make up the meshes and colliders we see on the scene.
        //Therefore what we understood here is that, when outHit detects a collider with tag "GreenHouse" it is actually the children objects.
        //So we got to route to the parent GreenHouse object that has this script to avoid interaction errors and to interact with the GreenHouse object properly.
        #endregion

        if (outHit.collider.tag == "Greenhouse" && 
            outHit.collider.transform.parent == this.transform && //Routing to parent transform...
            outHit.collider.transform.parent.GetComponent<GreenHouseManager>()._greenHouseOwnership == GreenHouseOwnership.Player) //Routing to parent transform...
        {
            //Debug.Log("outHit entered in GreenHouseManager!");
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfGreenHouseCommandsForPlayer);
            //Debug.Log("outHit Data loaded from GreenHouseManager!");
        }
        else if (outHit.collider.transform == null)
        {
           // Debug.Log("outHit entered in GreenHouseManager!");
            //Debug.LogError("_outHit is null in GreenHouseManager!");
        }
    }
}