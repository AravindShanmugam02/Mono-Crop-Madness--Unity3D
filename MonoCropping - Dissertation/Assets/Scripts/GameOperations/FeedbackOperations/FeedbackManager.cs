using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FeedbackManager : MonoBehaviour
{
    public void ActivateFeedbackScreen() { ToggleFeedbackPanel(true); }
    public bool GetIsFeedbackActive() { return _toggleFeedbackPanel; }

    [Header("Other Variables")]
    [SerializeField] private bool _toggleFeedbackPanel;

    [Header("Feedback Components")]
    [SerializeField] private GameObject _feedbackPanel;
    [SerializeField] private Button _feedbackPanelButton;
    [SerializeField] private TextMeshProUGUI _averageFarmingPatternOfLandsCaptionText;
    [SerializeField] private TextMeshProUGUI _averageFarmingPatternOfLandsValueText;
    [SerializeField] private TextMeshProUGUI _mostOfLastCultivatedCropsOfLandsCaptionText;
    [SerializeField] private TextMeshProUGUI _mostOfLastCultivatedCropsOfLandsValueText;
    [SerializeField] private TextMeshProUGUI _environmentHealthLevelCaptionText;
    [SerializeField] private Slider _environmentHealthLevelSlider;
    [SerializeField] private TextMeshProUGUI _environmentHealthLevelValueNumberText;
    [SerializeField] private TextMeshProUGUI _nutritionLevelCaptionText;
    [SerializeField] private Slider _nutritionLevelSlider;
    [SerializeField] private TextMeshProUGUI _nutritionLevelValueNumberText;
    [SerializeField] private TextMeshProUGUI _healthLevelCaptionText;
    [SerializeField] private Slider _healthLevelSlider;
    [SerializeField] private TextMeshProUGUI _healthLevelValueNumberText;
    [SerializeField] private TextMeshProUGUI _detectedFarmingPatternOfLandsCaptionText;
    [SerializeField] private TextMeshProUGUI _detectedFarmingPatternOfLandsValueText;
    [SerializeField] private TextMeshProUGUI _detectedFarmingPatternOfPlotsCaptionText;
    [SerializeField] private TextMeshProUGUI _detectedFarmingPatternOfPlotsValueText;
    [SerializeField] private TextMeshProUGUI _communityLevelCaptionText;
    [SerializeField] private Slider _communityLevelSlider;
    [SerializeField] private TextMeshProUGUI _communityLevelValueNumberText;
    [SerializeField] private TextMeshProUGUI _communityNutritionLevelCaptionText;
    [SerializeField] private Slider _communityNutritionLevelSlider;
    [SerializeField] private TextMeshProUGUI _communityNutritionLevelValueNumberText;
    [SerializeField] private TextMeshProUGUI _communityPopulationPercentageCaptionText;
    [SerializeField] private Slider _communityPopulationPercentageSlider;
    [SerializeField] private TextMeshProUGUI _communityPopulationPercentageValueNumberText;
    [SerializeField] private TextMeshProUGUI _populationCountCaptionText;
    [SerializeField] private Slider _populationCountSlider;
    [SerializeField] private TextMeshProUGUI _populationCountValueNumberText;
    [SerializeField] private List<TextMeshProUGUI> _communityFeedingStreak;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsCountCaptionText;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsCountValueText;
    [SerializeField] private TextMeshProUGUI _barnStorageItemsCountCaptionText;
    [SerializeField] private TextMeshProUGUI _barnStorageItemsCountValueText;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsTargetCountCaptionText;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsTargetCountValueText;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsSafeCountCaptionText;
    [SerializeField] private TextMeshProUGUI _barnStorageFoodItemsSafeCountValueText;
    [SerializeField] private TextMeshProUGUI _toolsCountFromInventoryCaptionText;
    [SerializeField] private TextMeshProUGUI _toolsCountFromInventoryValueText;
    [SerializeField] private TextMeshProUGUI _equipmentsCountFromInventoryCaptionText;
    [SerializeField] private TextMeshProUGUI _equipmentsCountFromInventoryValueText;
    [SerializeField] private TextMeshProUGUI _cropsCountFromInventoryCaptionText;
    [SerializeField] private TextMeshProUGUI _cropsCountFromInventoryValueText;
    [SerializeField] private TextMeshProUGUI _seedsCountFromInventoryCaptionText;
    [SerializeField] private TextMeshProUGUI _seedsCountFromInventoryValueText;

    private PauseManager _pauseManager;

    public static FeedbackManager Instance { get; private set; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        _pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();

        ToggleFeedbackPanel(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_toggleFeedbackPanel)
        {
            //ActivateFeedbackScreen(); -- Testing...

            //Take data from HUD to set with all the variables here...
            //This is in Update() function (instead of in ToggleFeebackPanel function) as to make the lerp or slerp work with slider values.
            FeedInTheDataToFeedbackScreen();
        }
        else
        {
            //Data Reset...
            ResetFeedbackScreenData();
        }
    }

    public void OnClickContinueButton()
    {
        #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
        ////To Prevent elements behind the current UI element to be interacted with mouse...
        ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

        /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
         * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
         * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
         * meaning, this method, which I've seen repeated... everywhere... 
         * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
        #endregion

        ToggleFeedbackPanel(false);
    }

    void ToggleFeedbackPanel(bool toggle)
    {
        _toggleFeedbackPanel = toggle;

        if (_toggleFeedbackPanel)
        {
            _feedbackPanel.SetActive(_toggleFeedbackPanel);
            _feedbackPanelButton.gameObject.SetActive(_toggleFeedbackPanel);
        }
        else
        {
            _feedbackPanel.SetActive(_toggleFeedbackPanel);
            _feedbackPanelButton.gameObject.SetActive(_toggleFeedbackPanel);
        }

        _pauseManager.SetPausedStatus(_toggleFeedbackPanel);
    }

    void FeedInTheDataToFeedbackScreen()
    {
        _averageFarmingPatternOfLandsValueText.text = HUDManager.Instance.GetAverageFarmingPatternOfLands();
        _mostOfLastCultivatedCropsOfLandsValueText.text = HUDManager.Instance.GetMostOfLastCultivatedCropsOfLands();

        //_environmentHealthLevelSlider.value = Mathf.Lerp(_environmentHealthLevelSlider.value, HUDManager.Instance.GetOverallEnvironmentHealthLevel(), 0.5f * Time.unscaledDeltaTime);
        if (_environmentHealthLevelSlider.value < HUDManager.Instance.GetOverallEnvironmentHealthLevel())
        {
            _environmentHealthLevelSlider.value += 1f;
        }
        else if (_environmentHealthLevelSlider.value >= HUDManager.Instance.GetOverallEnvironmentHealthLevel())
        {
            _environmentHealthLevelSlider.value = HUDManager.Instance.GetOverallEnvironmentHealthLevel();
        }
        _environmentHealthLevelValueNumberText.text = _environmentHealthLevelSlider.value.ToString("0.0") + "/" + _environmentHealthLevelSlider.maxValue;

        //_nutritionLevelSlider.value = Mathf.Lerp(_nutritionLevelSlider.value, HUDManager.Instance.GetOverallNutritionLevelOfAllPlots(), 0.5f * Time.unscaledDeltaTime);
        if (_nutritionLevelSlider.value < HUDManager.Instance.GetOverallNutritionLevelOfAllPlots())
        {
            _nutritionLevelSlider.value += 1f;
        }
        else if(_nutritionLevelSlider.value >= HUDManager.Instance.GetOverallNutritionLevelOfAllPlots())
        {
            _nutritionLevelSlider.value = HUDManager.Instance.GetOverallNutritionLevelOfAllPlots();
        }
        _nutritionLevelValueNumberText.text = _nutritionLevelSlider.value.ToString("0.0") + "/" + _nutritionLevelSlider.maxValue;

        //_healthLevelSlider.value = Mathf.Lerp(_healthLevelSlider.value, HUDManager.Instance.GetOverallHealthLevelOfAllPlots(), 0.5f * Time.unscaledDeltaTime);
        if (_healthLevelSlider.value < HUDManager.Instance.GetOverallHealthLevelOfAllPlots())
        {
            _healthLevelSlider.value += 1f;
        }
        else if (_healthLevelSlider.value >= HUDManager.Instance.GetOverallHealthLevelOfAllPlots())
        {
            _healthLevelSlider.value = HUDManager.Instance.GetOverallHealthLevelOfAllPlots();
        }
        _healthLevelValueNumberText.text = _healthLevelSlider.value.ToString("0.0") + "/" + _healthLevelSlider.maxValue;

        _detectedFarmingPatternOfLandsValueText.text = HUDManager.Instance.GetAverageFarmingPatternOfLands();
        _detectedFarmingPatternOfPlotsValueText.text = HUDManager.Instance.GetAverageFarmingPatternFromPlots();

        //_communityLevelSlider.value = Mathf.Lerp(_communityLevelSlider.value, HUDManager.Instance.GetOverallCommunityLevel(), 0.5f * Time.unscaledDeltaTime);
        if (_communityLevelSlider.value < HUDManager.Instance.GetOverallCommunityLevel())
        {
            _communityLevelSlider.value += 1f;
        }
        else if (_communityLevelSlider.value >= HUDManager.Instance.GetOverallCommunityLevel())
        {
            _communityLevelSlider.value = HUDManager.Instance.GetOverallCommunityLevel();
        }
        _communityLevelValueNumberText.text = _communityLevelSlider.value.ToString("0.0") + "/" + _communityLevelSlider.maxValue;

        //_communityNutritionLevelSlider.value = Mathf.Lerp(_communityNutritionLevelSlider.value, HUDManager.Instance.GetOverallCommunityNutritionLevel(), 0.5f * Time.unscaledDeltaTime);
        if (_communityNutritionLevelSlider.value < HUDManager.Instance.GetOverallCommunityNutritionLevel())
        {
            _communityNutritionLevelSlider.value += 1f;
        }
        else if (_communityNutritionLevelSlider.value >= HUDManager.Instance.GetOverallCommunityNutritionLevel())
        {
            _communityNutritionLevelSlider.value = HUDManager.Instance.GetOverallCommunityNutritionLevel();
        }
        _communityNutritionLevelValueNumberText.text = _communityNutritionLevelSlider.value.ToString("0.0") + "/" + _communityNutritionLevelSlider.maxValue;

        //_communityPopulationPercentageSlider.value = Mathf.Lerp(_communityPopulationPercentageSlider.value, HUDManager.Instance.GetOverallCommunityPopulationPercentage(), 0.5f * Time.unscaledDeltaTime);
        if (_communityPopulationPercentageSlider.value < HUDManager.Instance.GetOverallCommunityPopulationPercentage())
        {
            _communityPopulationPercentageSlider.value += 1f;
        }
        else if (_communityPopulationPercentageSlider.value >= HUDManager.Instance.GetOverallCommunityPopulationPercentage())
        {
            _communityPopulationPercentageSlider.value = HUDManager.Instance.GetOverallCommunityPopulationPercentage();
        }
        _communityPopulationPercentageValueNumberText.text = _communityPopulationPercentageSlider.value.ToString("0.0") + "/" + _communityPopulationPercentageSlider.maxValue;

        //_populationCountSlider.value = Mathf.Lerp(_populationCountSlider.value, HUDManager.Instance.GetPopulationCount(), 0.5f * Time.unscaledDeltaTime);
        if (_populationCountSlider.value < HUDManager.Instance.GetPopulationCount())
        {
            _populationCountSlider.value += 1;
            _populationCountSlider.maxValue = HUDManager.Instance.GetMaxPopulationCount();
        }
        else if (_populationCountSlider.value >= HUDManager.Instance.GetPopulationCount())
        {
            _populationCountSlider.value = HUDManager.Instance.GetPopulationCount();
            _populationCountSlider.maxValue = HUDManager.Instance.GetMaxPopulationCount();
        }
        _populationCountValueNumberText.text = _populationCountSlider.value.ToString() + "/" + _populationCountSlider.maxValue;

        //Community Feeding/Starving Streak
        List<bool> tmp = CommunityManager.Instance.GetCommunityFeedingStreak();
        for(int i = 0; i < _communityFeedingStreak.Count; i++)
        {
            _communityFeedingStreak[i].text = "-";
        }
        if(tmp != null && tmp.Count > 0)
        {
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i] == true)
                {
                    _communityFeedingStreak[i].text = "Yes";
                }
                else
                {
                    _communityFeedingStreak[i].text = "No";
                }
            }
        }

        _barnStorageFoodItemsCountValueText.text = HUDManager.Instance.GetOverallFoodCount().ToString();
        _barnStorageItemsCountValueText.text = HUDManager.Instance.GetOverallStorageCount().ToString();
        _barnStorageFoodItemsTargetCountValueText.text = HUDManager.Instance.GetBarnFoodItemsTargetCount().ToString();
        _barnStorageFoodItemsSafeCountValueText.text = HUDManager.Instance.GetBarnFoodItemsSafeCount().ToString();

        _toolsCountFromInventoryValueText.text = HUDManager.Instance.GetToolsCount().ToString();
        _equipmentsCountFromInventoryValueText.text = HUDManager.Instance.GetEquipmentsCount().ToString();
        _cropsCountFromInventoryValueText.text = HUDManager.Instance.GetCropsCount().ToString();
        _seedsCountFromInventoryValueText.text = HUDManager.Instance.GetSeedsCount().ToString();
    }

    void ResetFeedbackScreenData()
    {
        _averageFarmingPatternOfLandsValueText.text = "-";
        _mostOfLastCultivatedCropsOfLandsValueText.text = "-";

        _nutritionLevelSlider.value = 0f;
        _healthLevelSlider.value = 0f;

        _detectedFarmingPatternOfLandsValueText.text = "-";
        _detectedFarmingPatternOfPlotsValueText.text = "-";

        _communityLevelSlider.value = 0f;
        _communityNutritionLevelSlider.value = 0f;
        _communityPopulationPercentageSlider.value = 0f;

        _populationCountSlider.value = 0f;

        _barnStorageFoodItemsCountValueText.text = "0";
        _barnStorageItemsCountValueText.text = "0";
        _barnStorageFoodItemsTargetCountValueText.text = "0";
        _barnStorageFoodItemsSafeCountValueText.text = "0";

        _toolsCountFromInventoryValueText.text = "0";
        _equipmentsCountFromInventoryValueText.text = "0";
        _cropsCountFromInventoryValueText.text = "0";
        _seedsCountFromInventoryValueText.text = "0";
    }
}