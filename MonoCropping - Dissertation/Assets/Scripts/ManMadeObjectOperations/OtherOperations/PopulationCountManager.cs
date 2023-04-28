using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulationCountManager : MonoBehaviour
{
    public void SetStarvingStatus(bool toggle) { _isStarving = toggle; }
    //public void SetNutritionLessStatus(bool toggle) { _isNutritionLess = toggle; }
    public void SetSafeZoneStatus(bool toggle) { _isInSafeZone = toggle; }
    public int GetCurrentPopulationCount() { return _populationCount; }
    public int GetMaxPopulationCount() { return _maxPopulationCount; }

    //public void SetGameTypeToLoad(GameManager.GameType gameType) { _gameType = gameType; }

    //To get what type of game to load...
    [SerializeField] GameManager.GameType _gameType;

    [SerializeField] private int _populationCount;
    [SerializeField] private int _maxPopulationCount;
    [SerializeField] private int _minPopulationCount;
    [SerializeField] private bool _isStarving;
    //[SerializeField] private bool _isNutritionLess; //I don't need this feature now, maybe for future update.
    [SerializeField] private bool _isInSafeZone;

    TextMeshProUGUI _populationCountText;

    #region Awake - Singleton Monobehaviour
    public static PopulationCountManager Instance { get; private set; }

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

        _isStarving = false;
        //_isNutritionLess = false;
        _isInSafeZone = false;

        _populationCount = 100;
        _maxPopulationCount = 150;
        _minPopulationCount = 0;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _populationCountText = GameObject.Find("PopulationCountText").GetComponent<TextMeshProUGUI>();
        _populationCountText.text = "0/" + _maxPopulationCount;

        LoadPopulationCountAccordingToGameType();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStarving)
        {
            _isStarving = !_isStarving;
            ReducePopulationCount();
        }

        //if (_isNutritionLess)
        //{
        //    _isNutritionLess = !_isNutritionLess;
        //    ReducePopulationCount();
        //}

        if (_isInSafeZone)
        {
            _isInSafeZone = !_isInSafeZone;
            IncreasePopulationCount();
        }

        _populationCountText.text = _populationCount + "/" + _maxPopulationCount;
    }

    void LoadPopulationCountAccordingToGameType()
    {
        _gameType = GameManager.Instance.GetGameTypeToLoad();

        if (_gameType == GameManager.GameType.NewGame)
        {
            _populationCount = 100;
        }
        else if (_gameType == GameManager.GameType.LoadGame)
        {
            _populationCount = GameManager.Instance.GetPopulationCount();
        }
    }

    private void ReducePopulationCount()
    {
        //if(_populationCount >= 50) //This is the first check for seeing if 10 can be reduced from total count to avoid negative values.
        //{
            _populationCount -= 50;
        //}

        if(_populationCount <= _minPopulationCount) //This is just a double check for ruling out negative value.
        {
            _populationCount = _minPopulationCount;
        }
        //return true;
        //return false;
    }

    private bool IncreasePopulationCount()
    {
        if(_populationCount <= 150) //This is the first check for seeing if 10 can be added to total count to avoid threshold breach.
        {
            _populationCount += 10;
            return true;
        }

        if(_populationCount >= _maxPopulationCount) //This is just a double check for ruling out threshold breach.
        {
            _populationCount = _maxPopulationCount;
        }
        return false;
    }
}