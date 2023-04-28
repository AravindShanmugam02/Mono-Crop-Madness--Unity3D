using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class DTSCManager : MonoBehaviour, ITimer
{
    //DTSC Panel
    //Text Objects
    TextMeshProUGUI _timeText;
    TextMeshProUGUI _dateText;
    TextMeshProUGUI _seasonText;
    TextMeshProUGUI _climateText;
    TextMeshProUGUI _weatherText;
    TextMeshProUGUI _temperatureText;

    //Image Objects
    Image _seasonImage;
    Image _climateImage;
    Image _weatherImage;
    Image _temperatureImage;

    //To store information from GameTimer
    int _hours, _minutes, _date, _year;
    string _ampm, _month, _season, _climate, _weather, _temperature;
    Sprite _seasonDryIMG, _seasonWetIMG, _climateMuggyIMG, _climateWarmIMG, _climateOvercastIMG;
    Sprite _weatherDayClearIMG, _weatherNightClearIMG, _weatherDayCloudyIMG, _weatherNightCloudyIMG, 
        _weatherComfortableIMG, _weatherHumidIMG, _weatherRainyIMG, _weatherSunnyIMG;
    [SerializeField] Sprite _temperatureHotIMG, _temperatureWarmIMG, _temperatureColdIMG;
    [SerializeField] List<Sprite> _weatherIconSet00, _weatherIconSet01, _weatherIconSet02, _weatherIconSet03;

    bool canStartCoroutine = false;
    bool ranOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        TimeManager.Instance.AddAsITimerListener(this);

        //Text Objects...
        _timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
        _dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        _seasonText = GameObject.Find("SeasonText").GetComponent<TextMeshProUGUI>();
        _climateText = GameObject.Find("ClimateText").GetComponent<TextMeshProUGUI>();
        _weatherText = GameObject.Find("WeatherText").GetComponent<TextMeshProUGUI>();
        _temperatureText = GameObject.Find("TemperatureText").GetComponent<TextMeshProUGUI>();

        //Image Objects...
        _seasonImage = GameObject.Find("SeasonImage").GetComponent<Image>();
        _climateImage = GameObject.Find("ClimateImage").GetComponent<Image>();
        _weatherImage = GameObject.Find("WeatherImage").GetComponent<Image>();
        _temperatureImage = GameObject.Find("TemperatureImage").GetComponent<Image>();

        if (_timeText == null || _dateText == null || _seasonText == null || _weatherText == null || _climateText == null || _temperatureText == null || 
            _seasonImage == null || _climateImage == null || _weatherImage == null || _temperatureImage == null)
        {
            Debug.LogError("Error in finding gameobjects - DTSC Manager!");
        }

        //Can use a foreach loop and get the icons set accordingly in runtime using their names, but it will be performance costly.
        //Therefore directly setting the index icons using their respective indexes.
        //_seasonDryIMG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Sprites/Weather/Dry.jpg"); //Cloudy icon
        //_seasonWetIMG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Sprites/Weather/Wet.png"); //Rainfall

        //_temperatureHotIMG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Sprites/Weather/Hot.jpg");
        //_temperatureWarmIMG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Sprites/Weather/Warm.jpg");
        //_temperatureColdIMG = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Sprites/Weather/Cold.jpg");


        //_weatherIconSet00 = new List<Sprite>();
        //_weatherIconSet01 = new List<Sprite>();
        //_weatherIconSet02 = new List<Sprite>();
        //_weatherIconSet03 = new List<Sprite>();

        //var _spritesToLoad00 = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Textures/Sprites/Weather/Weather_Icons_00_Transparent.png");
        //foreach (var sprite in _spritesToLoad00)
        //{
        //    _weatherIconSet00.Add((sprite as Sprite));
        //}
        //var _spritesToLoad01 = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Textures/Sprites/Weather/Weather_Icons_04_Original.png");
        //foreach (var sprite in _spritesToLoad01)
        //{
        //    _weatherIconSet01.Add((sprite as Sprite));
        //}
        //var _spritesToLoad02 = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Textures/Sprites/Weather/Weather_Icons_05_Original.png");
        //foreach (var sprite in _spritesToLoad02)
        //{
        //    _weatherIconSet02.Add((sprite as Sprite));
        //}
        //var _spritesToLoad03 = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/Textures/Sprites/Weather/Weather_Icons_06_Original.png");
        //foreach (var sprite in _spritesToLoad02)
        //{
        //    _weatherIconSet03.Add((sprite as Sprite));
        //}

        if (_weatherIconSet00.Count != 0 && _weatherIconSet01.Count != 0 && _weatherIconSet02.Count != 0)
        {
            _climateMuggyIMG = _weatherIconSet01[1]; //01
            _climateWarmIMG = _weatherIconSet01[3]; //03
            _climateOvercastIMG = _weatherIconSet01[14]; //14

            _seasonDryIMG = _weatherIconSet00[17]; //17
            _seasonWetIMG = _weatherIconSet00[20]; //20

            _weatherDayClearIMG = _weatherIconSet00[0]; //00
            _weatherNightClearIMG = _weatherIconSet00[22]; //22

            _weatherDayCloudyIMG = _weatherIconSet00[3]; //03
            _weatherNightCloudyIMG = _weatherIconSet00[8]; //08

            _weatherComfortableIMG = _weatherIconSet00[13]; //13
            _weatherHumidIMG = _weatherIconSet00[4]; //4
            _weatherRainyIMG = _weatherIconSet02[4]; //4
            _weatherSunnyIMG = _weatherIconSet02[1]; //1
        }
        else
        {
            Debug.LogError("Error in loading sprites - DTSC Manager!");
        }

        //Setting image to hide in the begining...
        _seasonImage.gameObject.SetActive(false);
        _climateImage.gameObject.SetActive(false);
        _weatherImage.gameObject.SetActive(false);
        _temperatureImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Not starting the coroutine in Start as some values displayed in first frame using coroutine should be in place.
        if (canStartCoroutine == true && ranOnce == false) //Start the Coroutine once
        {
            StartCoroutine(DTSCPanelUpdate());
            ranOnce = true;
        }
    }

    public void TickUpdate(GameTimer gameTimer)
    {
        _hours = gameTimer.GetHours();
        _minutes = gameTimer.GetMinutes();
        _date = gameTimer.GetDate();
        _year = gameTimer.GetYear();

        _ampm = "AM"; //No need of a conditions for 24:00 because it will be converted to 00:00 in GameTimer.
        if (_hours > 12)
        {
            _ampm = "PM";
            _hours -= 12;
        }
        else if(_hours == 12){
            _ampm = "PM";
        }

        _month = gameTimer.GetMonth().ToString();
        _season = gameTimer.GetSeason().ToString();
        _climate = gameTimer.GetClimate().ToString();
        _weather = gameTimer.GetWeather().ToString();
        _temperature = gameTimer.GetTemperature().ToString();

        canStartCoroutine = true; //Start Coroutine after the first frame's values are in place.

        //Below two placed here instead of in Coroutine because these needs to be displayed everyframe.
        _timeText.text = _hours.ToString("00") + ":" + _minutes.ToString("00") + " " + _ampm.ToUpper();
        _dateText.text = _date.ToString("00") + " " + _month + " " + _year.ToString("0000");
    }

    IEnumerator DTSCPanelUpdate()
    {        
       while (true)
       {
            //Texts
            _seasonText.text = _season;
            _climateText.text = _climate;
            _weatherText.text = _weather;
            _temperatureText.text = _temperature;

            //Setting text to display...
            _seasonText.gameObject.SetActive(true);
            _climateText.gameObject.SetActive(true);
            _weatherText.gameObject.SetActive(true);
            _temperatureText.gameObject.SetActive(true);

            yield return new WaitForSeconds(3f);

            //Setting text off...
            _seasonText.gameObject.SetActive(false);
            _climateText.gameObject.SetActive(false);
            _weatherText.gameObject.SetActive(false);
            _temperatureText.gameObject.SetActive(false);

            //Images
            //Image for Season
            //Dry, Wet
            if (_season == "Dry")
            {
                _seasonImage.sprite = _seasonDryIMG;
                _seasonImage.preserveAspect = true;
            }
            else if (_season == "Wet")
            {
                _seasonImage.sprite = _seasonWetIMG;
                _seasonImage.preserveAspect = true;
            }

            //Image for Climate
            //Muggy, Warm, Overcast
            if (_climate == "Muggy")
            {
                _climateImage.sprite = _climateMuggyIMG;
                _climateImage.preserveAspect = true;
            }
            else if (_climate == "Warm")
            {
                _climateImage.sprite = _climateWarmIMG;
                _climateImage.preserveAspect = true;
            }
            else if (_climate == "Overcast")
            {
                _climateImage.sprite = _climateOvercastIMG;
                _climateImage.preserveAspect = true;
            }

            //Image for Weather
            //Clear, Cloudy, Comfortable, Humid, Rainy, Sunny
            switch (_weather)
            {
                case "Clear":
                    if (_ampm == "AM")
                    { _weatherImage.sprite = _weatherDayClearIMG; _weatherImage.preserveAspect = true; }
                    else
                    { _weatherImage.sprite = _weatherNightClearIMG; _weatherImage.preserveAspect = true; }
                    break;

                case "Cloudy":
                    if (_ampm == "AM")
                    { _weatherImage.sprite = _weatherDayCloudyIMG; _weatherImage.preserveAspect = true; }
                    else
                    { _weatherImage.sprite = _weatherNightCloudyIMG; _weatherImage.preserveAspect = true; }
                    break;

                case "Comfortable":
                    _weatherImage.sprite = _weatherComfortableIMG;
                    _weatherImage.preserveAspect = true;
                    break;

                case "Humid":
                    _weatherImage.sprite = _weatherHumidIMG;
                    _weatherImage.preserveAspect = true;
                    break;

                case "Rainy":
                    _weatherImage.sprite = _weatherRainyIMG;
                    _weatherImage.preserveAspect = true;
                    break;

                case "Sunny":
                    _weatherImage.sprite = _weatherSunnyIMG;
                    _weatherImage.preserveAspect = true;
                    break;

                default:
                    //Debug.Log("Error in DTSCManager - Try again with appropriate value!");
                    break;
            }

            //Image for temparature
            //Hot, Warm, Cold
            if (_temperature == "Hot")
            {
                _temperatureImage.sprite = _temperatureHotIMG;
                _temperatureImage.preserveAspect = true;
            }
            else if (_temperature == "Warm")
            {
                _temperatureImage.sprite = _temperatureWarmIMG;
                _temperatureImage.preserveAspect = true;
            }
            else if (_temperature == "Cold")
            {
                _temperatureImage.sprite = _temperatureColdIMG;
                _temperatureImage.preserveAspect = true;
            }

            //Setting image on...
            _seasonImage.gameObject.SetActive(true);
            _climateImage.gameObject.SetActive(true);
            _weatherImage.gameObject.SetActive(true);
            _temperatureImage.gameObject.SetActive(true);

            yield return new WaitForSeconds(3f);

            //Setting image to hide..
            _seasonImage.gameObject.SetActive(false);
            _climateImage.gameObject.SetActive(false);
            _weatherImage.gameObject.SetActive(false);
            _temperatureImage.gameObject.SetActive(false);
       }
    }
}