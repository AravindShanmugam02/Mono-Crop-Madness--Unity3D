using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkyManager : MonoBehaviour, ITimer
{
    #region Public Getters & Setters
    public void SetRainfallDuration(float duration) { _rainfallDuration = duration; }
    public float GetRainfallDuration() { return _rainfallDuration; }
    public void SetRainStatus(bool status) { _isRaining = status; }
    public bool GetRainStatus() { return _isRaining; }
    public void SetIsRainingFor6Hours(bool status) { _hasBeenFor6Hours = status; }
    public bool GetIsRainingFor6Hours() { return _hasBeenFor6Hours; }
    public void AddAsSkyInfoListener(SkyInfo listener) { RegisterAsSkyInfoListener(listener); }
    public void RemoveFromSkyInfoListener(SkyInfo listener) { UnregisterFromSkyInfoListeners(listener); }
    #endregion

    private GameObject _sun;
    [SerializeField] private GameObject _rainfallPrefab;
    private GameObject _splashPrefab;

    private GameTimer _gameTimer;

    private float _fromHour;
    private float _fromMinute;
    private float _rainfallDuration;
    private bool _canStart = false;
    private bool _ranOnce = false;

    private float _previousSunAngle;
    //private float _currentSunAngle;

    [SerializeField] private bool _isRaining = false;
    [SerializeField] private bool _hasBeenFor6Hours = false;

    private List<SkyInfo> _skyListenersList;

    
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static SkyManager Instance { get; private set; }

    //Making this class to be a singleton monobehaviour class - only one instace will be created for this class.
    private void Awake()
    {
        //if there is more than one instance, destroy the extras.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static Instance to this Instance.
            Instance = this;
        }

        _skyListenersList = new List<SkyInfo>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeManager.Instance.AddAsITimerListener(this);

        _sun = GameObject.Find("Directional Light").gameObject;
        if (!_sun)
        {
            Debug.LogError("Error - Sky Manager. Missing Sun!");
        }

        //_rainfallPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/3D/Nature/Weather/Rainfall.prefab", typeof(GameObject)) as GameObject;
        _splashPrefab = _rainfallPrefab?.transform.GetChild(0).gameObject;

        if (_rainfallPrefab == null || _splashPrefab == null)
        {
            Debug.LogError("Error in WeatherSystem - Rainfall Prefab or Splash Prefab is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameTimer != null)
        {
            UpdateSky();

            if (_canStart == true && _ranOnce == false)
            {
                InstantiateWeather();
            }
        }
    }

    void UpdateSky()
    {
        UpdateSun();
        UpdateRain();
    }

    void UpdateSun()
    {
        _previousSunAngle = _sun.transform.eulerAngles.x;
        float currentSunAngle = 0.00416f * _gameTimer.GetPresentDayTimeInSeconds() - 90;

        #region COMMENTS AND POTENTIAL FUTURE IMPROVEMENT
        //Didn't smooth movement. Need to find a way to smooth sun angle movement
        #endregion
        //_sun.transform.eulerAngles = Vector3.Slerp(new Vector3(_previousSunAngle, 270, 0), new Vector3(currentSunAngle, 270, 0), 1f);

        _sun.transform.eulerAngles = new Vector3(currentSunAngle, 270, 0);
    }

    void UpdateRain()
    {
        if(_isRaining == true)
        {
            foreach(SkyInfo skyInfo in _skyListenersList)
            {
                skyInfo.RainInfo(_isRaining, _hasBeenFor6Hours);
            }
        }
    }

    //Instantiating rainfall and stuff...
    public void UpdateWeather(bool monthSkipped = false) //Updates at start of every day -> 00:00
    {
        //..Nothing as of now
        if(_gameTimer.GetWeather().ToString() == "Rainy")
        {
            if (monthSkipped)
            {
                _fromHour = TimeManager.Instance.GetRandomHourInADayForSwitchedMonth();
                _fromMinute = TimeManager.Instance.GetRandomMinuteInAnHourForSwitchedMonth();
            }
            else
            {
                _fromHour = TimeManager.Instance.GetRandomHourInADay();
                _fromMinute = TimeManager.Instance.GetRandomMinuteInAnHour();
            }
            _canStart = true; _ranOnce = false;
        }
    }

    private void InstantiateWeather() //Updates every frame once the the Update weather is ran.
    {
        if (_gameTimer.GetHours() == _fromHour && _gameTimer.GetMinutes() == _fromMinute)
        {
            switch (_gameTimer.GetWeather().ToString())
            {
                case "Rainy":

                    Instantiate(_rainfallPrefab, _rainfallPrefab.transform.position, Quaternion.Euler(-90f, 0f, 0f), this.transform);
                    _canStart = false; _ranOnce = true; //Stops from running this method everyframe after the particle system is implemented.

                    break;

                default:

                    //Nothing...

                    break;
            }
        }
    }

    private void RegisterAsSkyInfoListener(SkyInfo listener)
    {
        _skyListenersList.Add(listener);
    }

    private void UnregisterFromSkyInfoListeners(SkyInfo listener)
    {
        _skyListenersList.Remove(listener);
    }

    public void TickUpdate(GameTimer gameTimer)
    {
        if (_gameTimer == null) { _gameTimer = gameTimer; }
    }
}