using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour, ITimer
{
    #region Public Getters & Setters
    public bool GetCropDestroyStatus() { return _cropDestroy; }
    public void SetCropDestroyStatus(bool value) { _cropDestroy = value; }
    public CropGrowthStage GetCropGrowthStage() { return _cropGrowthStage; }
    public CropItem GetCropItem() { return _cropItem; }
    #endregion

    #region Class Member Variables
    private LandManager _parentLand;
    private SeedItem _seedItem;
    private CropItem _cropItem;
    private GameObject _seed;
    private GameObject _seedling;
    private GameObject _yield;
    //private GameTimer _cropBehaviourOwnTimer;
    //private GameTimer _gameTimer;
    [SerializeField] private float _growthDuration;
    [SerializeField] private float _growthProgress = 0f;
    [SerializeField] private float _rainfallDuration;
    [SerializeField] private bool _cropDestroy = false;
    public enum CropGrowthStage
    {
        Seed, Seedling, Sapling, Mature, Flowering, Yield
    }
    [SerializeField] private CropGrowthStage _cropGrowthStage;

    private string _landOwnership;
    #endregion

    #region Default Functions of Monobehaviour

    // Start is called before the first frame update
    void Start()
    {
        //Get land reference by getcomponentinparent.
        _parentLand = GetComponentInParent<LandManager>();

        //Get land ownership from cultivated land.
        _landOwnership = _parentLand.GetLandOwnership().ToString();

        //Get Seed Item data from the land.
        if (_landOwnership == "AI") { _seedItem = _parentLand?.GetAICultivatedCrop(); }
        else { _seedItem = _parentLand?.GetEquipedItemCS() as SeedItem; }
        
        //Using the seed item data, we get which crop will be yielded as a result of this crop.
        _cropItem = _seedItem?._cropToYield;

        //Get the seed model from this prefab's children.
        _seed = this.transform.GetChild(0).gameObject;

        //Using the seed item data, we get the seedling model information.
        _seedling = _seedItem?._seedlingModel;

        //Using the crop item data, we get the yield model information, that is, the final crop which will be grown.
        _yield = _cropItem?._itemModel;

        if(_parentLand == null || _seedItem == null || _cropItem == null 
            || _seed == null || _seedling == null || _yield == null)
        {
            Debug.LogError("Error in Cropbehaviour :: " + this + " Null items!!");
        }

        TimeManager.Instance.AddAsITimerListener(this);

        _growthDuration = TimeManager.Instance.GetDaysInMinutes(_seedItem._daysToGrow);

        //Below is commented as the game timer updates 60seconds per second in real world...
        //_growthDuration *= 2; //Since my game timer updates half a minute every second from the start.

        _seedling = Instantiate(_seedling, this.transform);
        _seedling.SetActive(false);

        _yield = Instantiate(_yield, this.transform);
        _yield.SetActive(false);
    }

    void Update()
    {

    }

    #endregion

    #region Growth Tracker, Growth Calculator, Growth Visualizer

    #region Growth Tracker

    void GrowthTracker()
    {
        //Here we will increase the percentage of crop growth.
        //Check for rainfall duration.
        _rainfallDuration = SkyManager.Instance.GetRainfallDuration();

        //We will check what is the status of the land.
        string _landState = _parentLand.GetLandState().ToString();

        //If the land is in dry status, crops will be destroyed.
        if (_landState == LandManager.LandState.Dry.ToString())
        {
            _growthProgress += 0f; //growth stops...
        }

        //If the land is in muddy status, will stop the growth.
        else if (_landState == LandManager.LandState.Muddy.ToString())
        {
            _growthProgress += 0f; //growth stops...
        }

        //If the land is in clay status && if recent rainfall lasted equal to or more than 6 hours, will stop the growth. Else, crops will grow with 50% rate only.
        else if (_landState == LandManager.LandState.Clay.ToString())
        {
            if(_rainfallDuration >= 6f)
            {
                _growthProgress += 0f; //growth stops...
            }
            else
            {
                _growthProgress += 0.5f; //growth in 50% rate
            }
        }

        //If the land is in farm status, crops will grow with no issues.
        else if (_landState == LandManager.LandState.Farm.ToString())
        {
            _growthProgress++;
        }

        //If the land is in grassy status, crops will grow with 10% rate only.
        else if (_landState == LandManager.LandState.Grassy.ToString())
        {
            _growthProgress += 0.1f; //growth in 10% rate
        }

        GrowthCalculator();
        GrowthVisualizer();
    }

    #endregion

    #region Growth Calculator

    void GrowthCalculator()
    {
        //Seed, Seedling, Sapling, Mature, Flowering, Yield

        //Seed... 0-20%
        if (_growthProgress < (_growthDuration * 20 / 100))
        {
            //Nothing Happens...
            _cropGrowthStage = CropGrowthStage.Seed;
        }

        //Seedling... 20-40%
        if (_growthProgress >= (_growthDuration * 20 / 100) && _growthProgress < (_growthDuration * 40 / 100))
        {
            _cropGrowthStage = CropGrowthStage.Seedling;
        }

        //Sapling... 40-60%
        if (_growthProgress >= (_growthDuration * 40 / 100) && _growthProgress < (_growthDuration * 60 / 100))
        {
            _cropGrowthStage = CropGrowthStage.Sapling;
        }

        //Mature... 60-80%
        if (_growthProgress >= (_growthDuration * 60 / 100) && _growthProgress < (_growthDuration * 80 / 100))
        {
            _cropGrowthStage = CropGrowthStage.Mature;
        }

        //Flowering... 80-100%
        if (_growthProgress >= (_growthDuration * 80 / 100) && _growthProgress < (_growthDuration * 100 / 100))
        {
            _cropGrowthStage = CropGrowthStage.Flowering;
        }

        //Full growth... 100%
        if (_growthProgress >= (_growthDuration * 100 / 100))
        {
            _cropGrowthStage = CropGrowthStage.Yield;
        }
    }

    #endregion

    #region Growth Visualizer

    void GrowthVisualizer()
    {
        //Where we instantiate the crop parts based on crop growth stage...
        switch (_cropGrowthStage)
        {
            case CropGrowthStage.Seed: //No changes...

                break;

            case CropGrowthStage.Seedling:
                if(_seed.activeSelf == true)
                {
                    _seed.SetActive(false);
                    _seedling.SetActive(true);
                    _yield.SetActive(false);
                }
                break;

            case CropGrowthStage.Sapling:

                break;

            case CropGrowthStage.Mature:

                break;

            case CropGrowthStage.Flowering:

                break;

            case CropGrowthStage.Yield:
                if (_seedling.activeSelf == true)
                {
                    _seed.SetActive(false);
                    _seedling.SetActive(false);
                    _yield.SetActive(true);
                    _cropDestroy = true;
                }
                break;

            default:
                //Debug.Log("Default Option in " + this);
                break;
        }
    }

    #endregion

    #endregion

    #region Observer Design Pattern Functions

    //This is updated every second, not every frame...
    public void TickUpdate(GameTimer gameTimer)
    {
        //if(_gameTimer == null) { _gameTimer = gameTimer; }

        //if (_cropBehaviourOwnTimer == null && _gameTimer != null)
        //{
        //    _cropBehaviourOwnTimer = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);
        //}

        if (_cropDestroy == false)
        {
            GrowthTracker();
        }
        //else if(_cropDestroy == true)
        //{
        //    TimeManager.Instance.RemoveFromITimerListener(this);
        //    Destroy(this.gameObject);
        //}
    }

    #endregion
}
