using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandManagerHUD : MonoBehaviour
{
    public GameObject GetAffectedByOtherLandsIndicatorObject() { return _affectedByOtherLandsIndicator; }
    public GameObject GetAffectedByOtherLandsIndicatorTextObject() { return _affectedByOtherLandsIndicatorText; }
    public GameObject GetAffectingOtherLandsIndicatorObject() { return _affectingOtherLandsIndicator; }
    public GameObject GetAffectingOtherLandsIndicatorTextObject() { return _affectingOtherLandsIndicatorText; }
    public GameObject GetViewLandDetailsPanelObject() { return _viewLandDetailsPanel; }
    public GameObject GetViewLandDetailsBackButtonObject() { return _viewLandDetailsBackButton.gameObject; }
    public GameObject GetLandManagerHUDInteractTextObject() { return _landManagerHUDInteractText.gameObject; }
    public bool GetIsLandDetailsViewed() { return _isLandDetailsViewed; }

    [Header("Main Objects")]
    [SerializeField] GameObject _affectedByOtherLandsIndicator;
    [SerializeField] GameObject _affectingOtherLandsIndicator;
    [SerializeField] GameObject _affectedByOtherLandsIndicatorText;
    [SerializeField] GameObject _affectingOtherLandsIndicatorText;
    [SerializeField] GameObject _viewLandDetailsPanel;
    [SerializeField] Button _viewLandDetailsBackButton;

    [Header("Sub-Objects 1")]
    [SerializeField] TextMeshProUGUI _landManagerHUDInteractText;

    [Header("Sub-Objects 1")]
    [SerializeField] Slider _landHealthSlider;
    [SerializeField] TextMeshProUGUI _landHealthValueText;
    [SerializeField] Slider _landNutritionSlider;
    [SerializeField] TextMeshProUGUI _landNutritionValueText;
    [SerializeField] Slider _plotHealthSlider;
    [SerializeField] TextMeshProUGUI _plotHealthValueText;
    [SerializeField] Slider _plotNutritionSlider;
    [SerializeField] TextMeshProUGUI _plotNutritionValueText;

    [Header("Sub-Objects 2")]
    [SerializeField] TextMeshProUGUI _landPatternValueText;
    [SerializeField] TextMeshProUGUI _plotPatternValueText;
    [SerializeField] TextMeshProUGUI _avgPatternOfLandsInThisPlotValueText;
    [SerializeField] TextMeshProUGUI _isItAffectingOtherLandsValueText;
    [SerializeField] TextMeshProUGUI _isItAffectedByOtherLandsValueText;

    [Header("Sub-Objects 3")]
    [SerializeField] List<TextMeshProUGUI> _lastFiveCultivatedCropsList;
    [SerializeField] TextMeshProUGUI _landStateValueText;

    [Header("Other Varibales")]
    [SerializeField] bool _isLandDetailsViewed;

    // Start is called before the first frame update
    void Start()
    {
        _affectedByOtherLandsIndicator.SetActive(false);
        _affectingOtherLandsIndicator.SetActive(false);
        _landManagerHUDInteractText.gameObject.SetActive(false);
        _viewLandDetailsPanel.SetActive(false);
        _viewLandDetailsBackButton.gameObject.SetActive(false);

        _isLandDetailsViewed = false;
        _landManagerHUDInteractText.text = "Hold V to view more details about this land!";
    }

    // Update is called once per frame
    void Update()
    {
        if (_viewLandDetailsPanel.activeSelf)
        {
            _isLandDetailsViewed = true;
        }
        else
        {
            _isLandDetailsViewed = false;
        }
    }

    public void FeedInData(LandManager land)
    {
        _landHealthSlider.value = land.GetOverallLandHealth();
        _landHealthSlider.maxValue = 100f;
        _landHealthValueText.text = _landHealthSlider.value.ToString() + "/" + _landHealthSlider.maxValue.ToString("0.0");

        _landNutritionSlider.value = land.GetLandNutritionLevel();
        _landNutritionSlider.maxValue = 100f;
        _landNutritionValueText.text = _landNutritionSlider.value.ToString() + "/" + _landNutritionSlider.maxValue.ToString("0.0");

        _plotHealthSlider.value = land.transform.parent.GetComponent<PlotManager>().GetOverallPlotHealth();
        _plotHealthSlider.maxValue = 100f;
        _plotHealthValueText.text = _plotHealthSlider.value.ToString() + "/" + _plotHealthSlider.maxValue.ToString("0.0");

        _plotNutritionSlider.value = land.transform.parent.GetComponent<PlotManager>().GetOverallPlotNutritionLevel();
        _plotNutritionSlider.maxValue = 100f;
        _plotNutritionValueText.text = _plotNutritionSlider.value.ToString() + "/" + _plotNutritionSlider.maxValue.ToString("0.0");


        _landPatternValueText.text = land.GetLandFarmingPattern();
        if (_landPatternValueText.text == "Monocropping") { _landPatternValueText.color = Color.red; }
        else if (_landPatternValueText.text == "PotentialMonocropping") { _landPatternValueText.color = Color.yellow; }
        else if (_landPatternValueText.text == "NoPattern") { _landPatternValueText.color = Color.white; }
        else { _landPatternValueText.color = Color.green; }

        _plotPatternValueText.text = land.transform.parent.GetComponent<PlotManager>().GetFarmingPatternOfPlot();
        if (_plotPatternValueText.text == "SingleCropping") { _plotPatternValueText.color = Color.gray; }
        else if (_plotPatternValueText.text == "MixedCropping") { _plotPatternValueText.color = Color.blue; }
        else if (_plotPatternValueText.text == "NoPattern") { _plotPatternValueText.color = Color.white; }
        else { _plotPatternValueText.color = Color.green; }

        _avgPatternOfLandsInThisPlotValueText.text = land.transform.parent.GetComponent<PlotManager>().GetAveragePatternOfChildrenLand();
        if (_avgPatternOfLandsInThisPlotValueText.text == "Monocropping") { _avgPatternOfLandsInThisPlotValueText.color = Color.red; }
        else if (_avgPatternOfLandsInThisPlotValueText.text == "PotentialMonocropping") { _avgPatternOfLandsInThisPlotValueText.color = Color.yellow; }
        else if (_avgPatternOfLandsInThisPlotValueText.text == "NoPattern") { _avgPatternOfLandsInThisPlotValueText.color = Color.white; }
        else { _avgPatternOfLandsInThisPlotValueText.color = Color.green; }

        _isItAffectingOtherLandsValueText.text = land.GetIsSpreadingFromThis() ? "Yes" : "No";
        if(_isItAffectingOtherLandsValueText.text == "Yes") { _isItAffectingOtherLandsValueText.color = new Color(0.8018868f, 0.1550819f, 0.1550819f, 1f); }
        else { _isItAffectingOtherLandsValueText.color = Color.green; }

        _isItAffectedByOtherLandsValueText.text = land.GetHasSpreadedFromOthers() ? "Yes" : "No";
        if (_isItAffectedByOtherLandsValueText.text == "Yes") { _isItAffectedByOtherLandsValueText.color = new Color(0.8396226f, 0.5425968f, 0.1386169f, 1f); }
        else { _isItAffectedByOtherLandsValueText.color = Color.green; }

        for (int i = 0; i < _lastFiveCultivatedCropsList.Count; i++)
        {
            _lastFiveCultivatedCropsList[i].text = "";
        }
        for(int i = 0; i < land.GetLandFarmingCropsRecord().Count; i++)
        {
            _lastFiveCultivatedCropsList[i].text = land.GetLandFarmingCropsRecord()[i].ToString();
        }

        _landStateValueText.text = land.GetLandState().ToString();
    }

    public void ToggleLandDetailsPanel(bool toggle)
    {
        _viewLandDetailsBackButton.gameObject.SetActive(toggle);
        _viewLandDetailsPanel.SetActive(toggle);
    }
}
