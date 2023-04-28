using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommunityManager : MonoBehaviour
{
    public void SetIsCommunityFedToday(bool toggle)
    {
        _hasTheCommunityBeenFedToday = toggle;
        UpdateStarvingStreakNow(_hasTheCommunityBeenFedToday);
    }

    public void UpdateStarvingStreakNow(bool toggle) { UpdateStarvingStreak(toggle); }
    public List<bool> GetCommunityFeedingStreak() { return _communityFeedingStreak; }
    public float GetOverallCommunityLevel() { return _overallCommunityLevel; }
    public float GetOverallCommunityNutritionLevel() { return _communityNutritionLevel; }
    public float GetOverallCommunityPopulationPercentage() { return _communityPopulationPercentage; }

    //Crucial variables for community manager...
    [SerializeField] private bool _hasTheCommunityBeenFedToday;
    
    [SerializeField] private float _maxCommunityPopulationCount;
    [SerializeField] private float _communityPopulationPercentage;
    [SerializeField] private float _communityNutritionLevel;
    [SerializeField] private float _overallCommunityLevel;

    [SerializeField] private List<bool> _communityFeedingStreak;
    [SerializeField] private int _maxStreakHistory;

    //Farm Manager and Plot Manager variables...
    private GameObject _farmManager;
    [SerializeField] private int _numberOfPlotManagers;
    [SerializeField] private List<PlotManager> _plotManagersList;

    //Community Level HUD variables...
    /*[SerializeField] */private Slider _communityLevelSlider;
    /*[SerializeField] */private TextMeshProUGUI _communityLevelSliderHandleText;

    #region Awake - Singleton Monobehaviour
    public static CommunityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _communityNutritionLevel = 0f;
        _communityPopulationPercentage = 0f;
        _overallCommunityLevel = 0f;

        _communityFeedingStreak = new List<bool>();
        _maxStreakHistory = 7; //For a week, hence 7.

        _plotManagersList = new List<PlotManager>();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _farmManager = GameObject.Find("FarmManager");
        _numberOfPlotManagers = _farmManager.transform.childCount;

        for (int i = 0; i < _numberOfPlotManagers; i++)
        {
            _plotManagersList.Add(_farmManager.transform.GetChild(i).GetComponent<PlotManager>());
        }

        if (_plotManagersList.Count == 0)
        {
            Debug.LogError("_plotManagersList is null in Community Manager!");
        }

        _communityLevelSlider = GameObject.Find("CommunityLevelSlider").GetComponent<Slider>();
        _communityLevelSliderHandleText = _communityLevelSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (_communityLevelSlider == null || _communityLevelSliderHandleText == null)
        {
            Debug.LogError("_communityLevelSlider OR _communityLevelSliderHandleText is null in Community Manager!");
        }

        _maxCommunityPopulationCount = PopulationCountManager.Instance.GetMaxPopulationCount();

        StartCoroutine(UpdateOverallCommunityLevel());
    }

    // Update is called once per frame
    void Update()
    {
        _communityLevelSlider.value = _overallCommunityLevel;
        _communityLevelSliderHandleText.text = _overallCommunityLevel.ToString("0.0");
    }

    private void UpdateStarvingStreak(bool toggle)
    {
        _communityFeedingStreak.Add(toggle);

        if (_communityFeedingStreak.Count > _maxStreakHistory)
        {
            _communityFeedingStreak.RemoveAt(0);
        }

        UpdateStarvingStreak();
    }

    private void UpdateStarvingStreak()
    {
        //This is starving streak checker for min 2 days.
        //if (_communityFeedingStreak.Count >= 2)
        //{
        //    if (_communityFeedingStreak[_communityFeedingStreak.Count - 1] == false &&
        //    _communityFeedingStreak[_communityFeedingStreak.Count - 1] == _communityFeedingStreak[_communityFeedingStreak.Count - 2])
        //    {
        //        //Population count starts going down.
        //        //The population decreases and increases at every end of the day.
        //        PopulationCountManager.Instance.SetStarvingStatus(true);
        //    }
        //}

        //This is starving streak checker for min 1 day
        if (_communityFeedingStreak.Count > 0)
        {
            if (_communityFeedingStreak[_communityFeedingStreak.Count - 1] == false)
            {
                //Population count starts going down.
                //The population decreases and increases at every end of the day.
                PopulationCountManager.Instance.SetStarvingStatus(true);
            }
        }
    }

    IEnumerator UpdateOverallCommunityLevel()
    {
        while (true)
        {
            CheckForCommunityNutritionLevel();
            CheckForCommunityPopulationCount();
            EvaluateOverallCommunityLevel();

            yield return new WaitForSeconds(1f);
        }
    }

    void CheckForCommunityNutritionLevel()
    {
        _communityNutritionLevel = 0f;

        //Nutrition Level is going to be a live update with farming pattern data.
        for (int i = 0; i < _plotManagersList.Count; i++)
        {
            _communityNutritionLevel += _plotManagersList[i].GetOverallPlotNutritionLevel();
        }

        _communityNutritionLevel /= _numberOfPlotManagers;
    }

    void CheckForCommunityPopulationCount()
    {
        //Population Count is going to be a live update with population count manager.
        _communityPopulationPercentage = (PopulationCountManager.Instance.GetCurrentPopulationCount() / _maxCommunityPopulationCount) * 100;
    }

    void EvaluateOverallCommunityLevel()
    {
        _overallCommunityLevel = (_communityPopulationPercentage + _communityNutritionLevel) / 2;
    }
}
