using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnvironmentHealthManager : MonoBehaviour
{
    public float GetEnvironmentHealthLevel() { return _environmentHealthLevel; }

    //Community Manager variables...
    private float _communityLevel;

    //Environment Health Level variable...
    [SerializeField] private float _environmentHealthLevel;
    
    //Farm Manager and Plot Manager variables...
    private GameObject _farmManager;
    [SerializeField] private float _overallPlotHealth;
    [SerializeField] private int _numberOfPlotManagers;
    [SerializeField] private List<PlotManager> _plotManagersList;

    //Health HUD variables...
    /*[SerializeField] */private Slider _environmentHealthSlider;
    /*[SerializeField] */private TextMeshProUGUI _environmentHealthSliderHandleText;

    // Start is called before the first frame update
    void Start()
    {
        _farmManager = GameObject.Find("FarmManager");
        _numberOfPlotManagers = _farmManager.transform.childCount;
        _plotManagersList = new List<PlotManager>();

        for (int i = 0; i < _numberOfPlotManagers; i++)
        {
            _plotManagersList.Add(_farmManager.transform.GetChild(i).GetComponent<PlotManager>());
        }

        if (_plotManagersList.Count == 0)
        {
            Debug.LogError("_plotManagersList is null in Environment Health Manager!");
        }

        _environmentHealthSlider = GameObject.Find("EnvironmentHealthSlider").GetComponent<Slider>();
        _environmentHealthSliderHandleText = _environmentHealthSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (_environmentHealthSlider == null || _environmentHealthSliderHandleText == null)
        {
            Debug.LogError("_environmentHealthSlider OR _environmentHealthSliderHandleText is null in Environment Health Manager!");
        }

        StartCoroutine(UpdateOverallEnvironmentHealthLevel());
    }

    // Update is called once per frame
    void Update()
    {
        _environmentHealthSlider.value = _environmentHealthLevel;
        _environmentHealthSliderHandleText.text = _environmentHealthLevel.ToString("0.0");
    }

    IEnumerator UpdateOverallEnvironmentHealthLevel()
    {
        while (true)
        {
            CheckForCommunityLevel();
            CheckForOverallPlotHealth();
            EvaluateEnvironmentHealthLevelLevel();

            yield return new WaitForSeconds(1f);
        }
    }

    void CheckForCommunityLevel()
    {
        _communityLevel = CommunityManager.Instance.GetOverallCommunityLevel();
    }

    void CheckForOverallPlotHealth()
    {
        _overallPlotHealth = 0f;

        //Nutrition Level is going to be a live update with farming pattern data.
        for (int i = 0; i < _plotManagersList.Count; i++)
        {
            _overallPlotHealth += _plotManagersList[i].GetOverallPlotHealth();
        }

        _overallPlotHealth /= _numberOfPlotManagers;
    }

    void EvaluateEnvironmentHealthLevelLevel()
    {
        _environmentHealthLevel = (_communityLevel + _overallPlotHealth) / 2;
    }
}
