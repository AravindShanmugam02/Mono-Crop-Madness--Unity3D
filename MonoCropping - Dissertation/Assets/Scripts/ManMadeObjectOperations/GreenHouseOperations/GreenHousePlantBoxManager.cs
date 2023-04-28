using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHousePlantBoxManager : MonoBehaviour
{
    public SeedItem GetCurrentCultivatedPlant() { return _currentPlantSeedItem; }
    public int GetQuantityOfItemsToHarvest() { return _quantityOfItemsToHarvest; }
    public bool GetIsPlantBoxCultivated() { return _isPlanted; }
    public bool GetIsPlantBoxGrowing() { return _isGrowing; }
    public bool GetIsPlantBoxReadyForHarvest() { return _isReadyForHarvest; }
    public bool CheckTheLastPlantedCropIsSame(SeedItem seed) { return CheckIfTheLastCropInThisPLantBoxIsSame(seed._cropToYield._itemName); }
    public void SetPlantBoxNumber(string number) { _plantBoxNumberInString = number; }

    //String to store the number of plant box...
    private string _plantBoxNumberInString;

    //To access and handle planting and harvesting system for this plant box...
    private GreenHousePlantGrowingSystem _plantBoxGrowingSystem;
    [SerializeField] private bool _isPlanted;
    [SerializeField] private bool _isGrowing;
    [SerializeField] private bool _isReadyForHarvest;
    [SerializeField] private List<string> _historyOfPlantedCrops;
    [SerializeField] private int _maxHistoryToStore;

    [SerializeField] private SeedItem _currentPlantSeedItem;
    [SerializeField] private float _daysRequiredToYield;
    [SerializeField] private int _quantityOfItemsToHarvest;

    // Start is called before the first frame update
    void Start()
    {
        _plantBoxGrowingSystem = transform.GetComponent<GreenHousePlantGrowingSystem>();

        _historyOfPlantedCrops = new List<string>();
        _maxHistoryToStore = 5;

        _currentPlantSeedItem = null;
        _daysRequiredToYield = 0;
        _quantityOfItemsToHarvest = 10;

        _isPlanted = false;
        _isReadyForHarvest = false;
        _isGrowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        _isPlanted = _plantBoxGrowingSystem.GetIsPlanted();
        _isGrowing = _plantBoxGrowingSystem.GetIsGrowing();
        _isReadyForHarvest = _plantBoxGrowingSystem.GetIsReadyForHarvest();
    }

    bool CheckIfTheLastCropInThisPLantBoxIsSame(string name)
    {
        if (_historyOfPlantedCrops.Count != 0 && _historyOfPlantedCrops[_historyOfPlantedCrops.Count - 1] == name)
        {
            return true;
        }
        return false;
    }

    void UpdatePlantedCropsHistory(string name)
    {
        _historyOfPlantedCrops.Add(name);
        RevisePlantedCropsHistory();
    }

    void RevisePlantedCropsHistory()
    {
        if (_historyOfPlantedCrops.Count > _maxHistoryToStore)
        {
            _historyOfPlantedCrops.RemoveAt(0);
        }
    }

    public void PlantInThisBox(SeedItem seed)
    {
        StartCoroutine(StartPlantGrowthSystem(seed));
    }

    IEnumerator StartPlantGrowthSystem(SeedItem seed)
    {
        _daysRequiredToYield = seed._daysToGrow;
        _plantBoxGrowingSystem.LoadDataInThisPlantBoxGrowingSystem(seed);

        //https://docs.unity3d.com/ScriptReference/WaitUntil.html
        //https://forum.unity.com/threads/setting-system-function-bool.623533/#post-4176481
        yield return new WaitUntil(() => _isGrowing == true);

        _currentPlantSeedItem = seed;
        _quantityOfItemsToHarvest = 10;
        UpdatePlantedCropsHistory(seed._cropToYield._itemName);
    }

    public void HarvestThisBox(int quantityToHarvestNow)
    {
        StartCoroutine(StartHarvestingProcess(quantityToHarvestNow));
    }

    IEnumerator StartHarvestingProcess(int quantityToHarvestNow)
    {
        _quantityOfItemsToHarvest -= quantityToHarvestNow;
        
        ValidateHarvestQuantity();

        yield return null;
    }

    void ValidateHarvestQuantity()
    {
        if(_quantityOfItemsToHarvest <= 0)
        {
            _plantBoxGrowingSystem.ResetDataInThisPlantBoxGrowingSystem();

            _currentPlantSeedItem = null;
            _daysRequiredToYield = 0;
        }
    }
}