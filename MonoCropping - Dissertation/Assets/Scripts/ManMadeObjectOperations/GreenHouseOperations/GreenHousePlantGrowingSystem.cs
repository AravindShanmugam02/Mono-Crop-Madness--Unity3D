using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHousePlantGrowingSystem : MonoBehaviour, ITimer
{
    public void LoadDataInThisPlantBoxGrowingSystem(SeedItem seed) { LoadDataForGrowingSystem(seed); }
    public void ResetDataInThisPlantBoxGrowingSystem() { ResetDataOfGrowingSystem(); }
    public bool GetIsPlanted() { return _isPlanted; }
    public bool GetIsGrowing() { return _isGrowing; }
    public bool GetIsReadyForHarvest() { return _isReadyForHarvest; }
    public void SetIsPlanted(bool toggle) { _isPlanted = toggle; }
    public void SetIsReadyForHarvest(bool toggle) { _isReadyForHarvest = toggle; }

    public enum PlantGrowthStagesInPlantBox
    {
        Seed, Seedling, Sapling, Mature, Flowering, Yield
    }
    [SerializeField] private PlantGrowthStagesInPlantBox _plantGrowthStage;

    [SerializeField] private float _growthDuration;
    [SerializeField] private float _growthProgress;

    [SerializeField] private string _currentPlantName;
    [SerializeField] private SeedItem _currentPlantSeedItem;

    [SerializeField] private bool _isPlanted;
    [SerializeField] private bool _isGrowing;
    [SerializeField] private bool _isReadyForHarvest;

    // Start is called before the first frame update
    void Start()
    {
        _currentPlantSeedItem = null;
        _currentPlantName = "";

        _growthDuration = 0f;
        _growthProgress = 0f;

        _isPlanted = false;
        _isGrowing = false;
        _isReadyForHarvest = false;

        TimeManager.Instance.AddAsITimerListener(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadDataForGrowingSystem(SeedItem seed)
    {
        _currentPlantSeedItem = seed;
        _currentPlantName = seed._cropToYield._itemName;

        _growthDuration = TimeManager.Instance.GetDaysInMinutes(seed._daysToGrow);
        _growthProgress = 0f;

        _isPlanted = true;
        _isGrowing = true;
        _isReadyForHarvest = false;

        //Debug.Log("Data Loaded For Growing!");
    }

    void ResetDataOfGrowingSystem()
    {
        _currentPlantSeedItem = null;
        _currentPlantName = "";

        _growthDuration = 0f;
        _growthProgress = 0f;

        _isPlanted = false;
        _isGrowing = false;
        _isReadyForHarvest = false;

        //Debug.Log("Data Reset For Growing!");
    }

    #region Plant Box - Growth Tracker & Growth Calculator. No Growth Visualizer For Plant Box

    void GrowthTracker()
    {
        //Here we will increase the percentage of crop growth.
        _growthProgress++;

        GrowthCalculator();
    }

    void GrowthCalculator()
    {
        //Seed, Seedling, Sapling, Mature, Flowering, Yield

        //Seed... 0-20%
        if (_growthProgress < (_growthDuration * 20 / 100))
        {
            //Nothing Happens...
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Seed;
        }

        //Seedling... 20-40%
        if (_growthProgress >= (_growthDuration * 20 / 100) && _growthProgress < (_growthDuration * 40 / 100))
        {
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Seedling;
        }

        //Sapling... 40-60%
        if (_growthProgress >= (_growthDuration * 40 / 100) && _growthProgress < (_growthDuration * 60 / 100))
        {
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Sapling;
        }

        //Mature... 60-80%
        if (_growthProgress >= (_growthDuration * 60 / 100) && _growthProgress < (_growthDuration * 80 / 100))
        {
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Mature;
        }

        //Flowering... 80-100%
        if (_growthProgress >= (_growthDuration * 80 / 100) && _growthProgress < (_growthDuration * 100 / 100))
        {
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Flowering;
        }

        //Full growth... 100%
        if (_growthProgress >= (_growthDuration * 100 / 100))
        {
            _plantGrowthStage = PlantGrowthStagesInPlantBox.Yield;

            _isGrowing = false;
            _isReadyForHarvest = true;
        }
    }


    #endregion

    public void TickUpdate(GameTimer gameTimer)
    {
        if(_isGrowing)
        {
            GrowthTracker();
        }
    }
}
