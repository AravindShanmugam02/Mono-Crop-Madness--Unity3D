using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //For using c#'s random.
using UnityEngine.Subsystems;

//Weather can change from minute-to-minute, hour-to-hour, day-to-day, and season-to-season.
//Climate, however, is the average of weather over time and space.
//An easy way to remember the difference is that climate is what you expect, like a very hot summer, and weather is what you get, like a hot day with pop-up thunderstorms.

//Weather data collected from -> Book: 'Colombia' by Sarah De Capua, 2004
//            ''              -> Weather_Report_Site: 'WeatherSpark.com' by Cedar Lake Ventures, Inc. A small company based in the Minneapolis area.

[System.Serializable]
public class GameTimer
{
    #region Public Getters & Setters
    public int GetMinutes() { return _minutes; }
    public int GetHours() { return _hours; }
    public PartOfTheDay GetPartOfTheDay() { return _partOfTheDay_Auto; }
    public DayOfWeek GetDayOfWeek() { return _dayOfWeek; }
    public int GetDate() { return _day; }
    public Temperature GetTemperature() { return _temperature; }
    public Weather GetWeather() { return _weather; }
    public Climate GetClimate() { return _climate; }
    public Month GetMonth() { return _customMonth; }
    public Season GetSeason() { return _season; }
    public int GetYear() { return _year; }

    public float GetPresentDayTimeInSeconds() { return PresentDayTimeInSeconds(); }
    public float GetDaysToHours(float days) { return DaysToHours(days); }
    public float GetHoursToMinutes(float hours) { return HoursToMinutes(hours); }
    public float GetNoOfDaysPassed(GameTimer g) { return DaysPassed(g); }
    public float GetNoOfHoursPassed(GameTimer g) { return HoursPassed(g); }
    public float GetNoOfMinutesPassed(GameTimer g) { return MinutesPassed(g); }
    #endregion

    #region Class Members
    [Header("GameTimer")]
    [SerializeField] private int _year;                                                                             //YEAR
    public enum Season
    {
        Dry, /*Dec-Mar & Jul-Sep*/
        Wet /*Oct-Nov & Apr-Jun*/
    }
    [SerializeField] private Season _season;                                                                        //SEASON
    public enum Climate
    {
        Muggy, Warm, Overcast
    }
    [SerializeField] private Climate _climate;                                                                      //CLIMATE
    public enum Month
    {
        Jan, //Muggy
        Feb, //Muggy
        Mar, //Overcast
        Apr, //Warm
        May, //Muggy
        Jun, //Warm
        Jul, //Warm
        Aug, //Warm
        Sep, //Muggy
        Oct, //Muggy
        Nov, //Overcast
        Dec  //Warm
    }
    [SerializeField] private Month _customMonth;                                                                    //CUSTOM_MONTH
    //This is for rainy seasons where when I switch to a rainy month, even if the forcast show as rainy day, rain won't pour, because the rain data gets generated for the 
    //day at 12AM (start of the calendar date). Thus, this variable will be used to check wheather the previous month is exactly the previous month of the switched 
    //rainy month. If it is exactly the previous month, then we know the time hasn't skipped, so eventually the rain data will be generated. But if not, We generate the 
    //rain details for the switched day (which missed to get rain data for the day (if the day is rainy day)).
    [SerializeField] private Month _previousMonth;                                                                  //PREVIOUS_MONTH
    [SerializeField] private Month _autoMonth;                                                                      //AUTO_MONTH
    public enum Weather
    {
        Clear, Cloudy, Comfortable, Humid, Rainy, Sunny
    }
    [SerializeField] private Weather _weather;                                                                      //WEATHER
    public enum WindStatus
    {
        Calmer, Windier
    }
    [SerializeField] private WindStatus _wind;                                                                      //WIND
    public enum WindDirection
    {
        from_East, from_South
    }
    [SerializeField] private WindDirection _windDirection;                                                          //WIND_DIRECTION
    [SerializeField] private float _windForcePercentage;                                                            //WIND_FORCE
    [SerializeField] private int _day;                                                                              //DAY
    public enum DayOfWeek
    {
        Sun, Mon, Tue, Wed, Thu, Fri, Sat
    }
    [SerializeField] private DayOfWeek _dayOfWeek;                                                                  //DAY_OF_WEEK
    public enum PartOfTheDay
    {
        EarlyMorning, //3AM - 6AM
        Morning, //7AM - 11AM
        Noon, //12PM
        Afternoon, //1PM - 4PM
        Evening, //5PM - 7PM
        Night, //8PM - 11PM
        MidNight //12AM - 2AM
    }
    [SerializeField] private PartOfTheDay _partOfTheDay_Auto;                                                       //PART_OF_THE_DAY_AUTO
    [SerializeField] private PartOfTheDay _partOfTheDay_Custom;                                                     //PART_OF_THE_DAY_CUSTOM
    [SerializeField] private PartOfTheDay _previousPartOfTheDay_Custom;                                             //PREVIOUS_PART_OF_THE_DAY_CUSTOM
    public enum Temperature
    {
        Hot, Warm, Cold
    }
    [SerializeField] private Temperature _temperature;                                                              //TEMPERATURE
    [SerializeField] private int _hours;                                                                            //HOURS
    [SerializeField] private int _minutes;                                                                          //MINUTES
    [SerializeField] private int _seconds;                                                                          //SECONDS

    [Header("Test")]
    [SerializeField] private float _daysPassed;
    [SerializeField] private float _hoursPassed;
    [SerializeField] private float _minutesPassed;

    private int _randomValueTemp;
    private int _randomValueWeather;

    //https://stackoverflow.com/a/69115961/18762063 & https://stackoverflow.com/a/69115997/18762063
    //System.Random is different from UnityEngine.Random. With System.Random we can create an instance, but cannot with UnityEngine.Random.
    private System.Random _rand;
    #endregion

    #region Class Constructors
    //Normal parameterised constructor
    public GameTimer(int year, Season season, Month month, int day, DayOfWeek dayOfWeek, int hours, int minutes, int seconds) //No void for constructor
    {
        _year = year;
        _season = season;
        _customMonth = month;
        _previousMonth = _customMonth;
        _autoMonth = _customMonth;
        _day = day;
        _dayOfWeek = dayOfWeek;
        _hours = hours;
        _minutes = minutes;
        _seconds = seconds;
        _partOfTheDay_Auto = SetPartOfTheDayWithHoursValue(hours);
        _partOfTheDay_Custom = _partOfTheDay_Auto;
        _previousPartOfTheDay_Custom = _partOfTheDay_Custom;

        _rand = new System.Random();
    }


    //Example to understand a copy constructor: https://www.geeksforgeeks.org/c-sharp-copy-constructor/
    //A Copy Constructor for creating a clone instance to get time for each objects in game. Will be used going further.
    public GameTimer(GameTimer gameTimer)
    {// If error occurs here, it might be a reason that gameTimer is null and it doesn't have any instance stored in it. Check if gameTimer has a value (or an instance assigned to it).
        _year = gameTimer._year;
        _season = gameTimer._season;
        _customMonth = gameTimer._customMonth;
        _previousMonth = _customMonth;
        _autoMonth = _customMonth;
        _day = gameTimer._day;
        _dayOfWeek = gameTimer._dayOfWeek;
        _hours = gameTimer._hours;
        _minutes = gameTimer._minutes;
        _seconds = gameTimer._seconds;
        _partOfTheDay_Auto = SetPartOfTheDayWithHoursValue(gameTimer._hours);
        _partOfTheDay_Custom = _partOfTheDay_Auto;
        _previousPartOfTheDay_Custom = _partOfTheDay_Custom;
    }
    #endregion

    #region Game Timer System
    public void UpdateTime()
    {
        //if (Input.GetKey(KeyCode.P))
        //{
        //    _seconds += 60; //for testing...
        //}
        //else
        //{
            _seconds += 60; //_seconds += 30;                                                                       //SECOND UPDATE
        //}

        //At the end of a minute
        if (_seconds >= 60)                                                                                         //MINUTE UPDATE
        {
            _seconds = 0;
            _minutes++;
        }

        //At the end of an hour
        if(_minutes >= 60)                                                                                          //HOURS UPDATE
        {
            _minutes = 0;
            _hours++;
        }

        //At the end of a day
        if(_hours >= 24)                                                                                            //DAY UPDATE
        {
            _hours = 0;
            _day++;
            SkyManager.Instance.UpdateWeather();
            _randomValueTemp = _rand.Next(1, 20);                                                                   //GENERATE RANDOM VALUES FOR TEMPERATURE
            _randomValueWeather = _rand.Next(1, 30);                                                                //GENERATE RANDOM VALUES FOR WEATHER
        }

        if(_previousPartOfTheDay_Custom != _partOfTheDay_Custom)
        {
            _previousPartOfTheDay_Custom = _partOfTheDay_Custom;

            switch (_partOfTheDay_Custom)
            {
                case PartOfTheDay.EarlyMorning:
                    _hours = 3;
                    break;
                case PartOfTheDay.Morning:
                    _hours = 7;
                    break;
                case PartOfTheDay.Noon:
                    _hours = 12;
                    break;
                case PartOfTheDay.Afternoon:
                    _hours = 13;
                    break;
                case PartOfTheDay.Evening:
                    _hours = 17;
                    break;
                case PartOfTheDay.Night:
                    _hours = 20;
                    break;
                case PartOfTheDay.MidNight:
                    _hours = 0;
                    break;
                default:
                    _hours = 7;
                    break;
            }
        }

        _partOfTheDay_Auto = SetPartOfTheDayWithHoursValue(_hours);

        switch (_customMonth)                                                                                             //MONTH UPDATE
        {
            case Month.Jan://-------------------------------------------------------------------------JANUARY-------------------------------------------------
                
                if(_previousMonth != _customMonth)
                {
                    _day = 1;
                    _previousMonth = _customMonth;
                }

                //Below 3 var are set here cuz these values shouldn't be null or garbage for the first run.
                //Will be seeing similarly for every month below.
                _season = Season.Dry; //Dec - Mar                                                                   //SEASON - FOR DEC-MAR
                _climate = Climate.Muggy;                                                                           //CLIMATE - FOR JAN
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR JAN
                _windDirection = WindDirection.from_East;                                                           //WIND_DIRECTION - FOR JAN

                if(_randomValueTemp <= 12) { _temperature = Temperature.Hot; }      //Least Wet (60-40)             //TEMPERATURE UPDATE - FOR EACH DAY IN JAN
                else { _temperature = Temperature.Warm; }

                //Cuz Humid is Hot are part of muggy climate. So kept Cloudy as Warm. Other months will follow this way of logic.
                if (_temperature == Temperature.Hot) { _weather = Weather.Humid; }
                else if(_temperature == Temperature.Warm) { _weather = Weather.Cloudy; }                            //WEATHER UPDATE - FOR EACH DAY IN JAN

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    //Below 3 var are updated here cuz when the month gets updated, corresponding values should also update on the same frame without delay.
                    //Will be seeing similarly for every month below.
                    _climate = Climate.Muggy;                                                                       //CLIMATE UPDATE - FOR FEB
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR FEB
                    _windDirection = WindDirection.from_East;                                                       //WIND_DIRECTION UPDATE - FOR FEB
                }

            break;

            case Month.Feb://-------------------------------------------------------------------------FEBRUARY-------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Dec - Mar                                                                   //SEASON - FOR DEC-MAR
                _climate = Climate.Muggy;                                                                           //CLIMATE - FOR FEB
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR FEB
                _windDirection = WindDirection.from_East;                                                           //WIND_DIRECTION - FOR FEB

                if (_randomValueTemp <= 12) { _temperature = Temperature.Hot; }     //60-40                         //TEMPERATURE UPDATE - FOR EACH DAY IN FEB
                else { _temperature = Temperature.Warm; }

                //Cuz Humid is Hot are part of muggy climate. So kept Cloudy as Warm.
                if (_temperature == Temperature.Hot) { _weather = Weather.Humid; }
                else if (_temperature == Temperature.Warm) { _weather = Weather.Cloudy; }                           //WEATHER UPDATE - FOR EACH DAY IN FEB

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_year % 4 == 0) //Leap Year
                {
                    if (_day > 29)
                    {
                        _day = 1;

                        _previousMonth = _customMonth;
                        _customMonth++;
                        _autoMonth = _customMonth;

                        _climate = Climate.Overcast;                                                                //CLIMATE UPDATE - FOR MAR (Leap Year)
                        _wind = WindStatus.Calmer;                                                                  //WIND_STATUS UPDATE - FOR MAR
                        _windDirection = WindDirection.from_South;                                                  //WIND_DIRECTION UPDATE - FOR MAR
                    }
                }
                else
                {
                    if (_day > 28)
                    {
                        _day = 1;

                        _previousMonth = _customMonth;
                        _customMonth++;
                        _autoMonth = _customMonth;

                        _climate = Climate.Overcast;                                                                //CLIMATE UPDATE - FOR MAR
                        _wind = WindStatus.Calmer;                                                                  //WIND_STATUS UPDATE - FOR MAR
                        _windDirection = WindDirection.from_South;                                                  //WIND_DIRECTION UPDATE - FOR MAR
                    }
                }

            break;

            case Month.Mar://-------------------------------------------------------------------------MARCH---------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Dec - Mar                                                                   //SEASON - FOR DEC-MAR
                _climate = Climate.Overcast;                                                                        //CLIMATE - FOR MAR
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR MAR
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR MAR

                //https://realonomics.net/what-is-overcast-weather/ & https://www.thoughtco.com/overcast-sky-definition-3444114
                //There are high chances for rain or snow during overcast. Overcast can be dark, cold and gloomy or just quiet and calm.
                if (_randomValueTemp <= 14) { _temperature = Temperature.Cold; }    //Overcast (70-30)              //TEMPERATURE UPDATE - FOR EACH DAY IN MAR
                else { _temperature = Temperature.Warm; }

                //When Cold, the weather is either Cloudy or Rainy with 70-30 ratio;
                if (_temperature == Temperature.Cold) { if (_randomValueWeather <= 21) { _weather = Weather.Cloudy; } else { _weather = Weather.Rainy; } }
                else if (_temperature == Temperature.Warm) { _weather = Weather.Humid; }                           //WEATHER UPDATE - FOR EACH DAY IN MAR

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    //_season = Season.Wet; //Apr - Jun                                                             //SEASON UPDATE - WET
                    _climate = Climate.Warm;                                                                        //CLIMATE UPDATE - FOR APR
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR APR
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR APR
                }

            break;

            case Month.Apr://-------------------------------------------------------------------------APRIL---------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Wet; //Apr - Jun                                                                   //SEASON - FOR APR-JUN
                _climate = Climate.Warm;                                                                            //CLIMATE - FOR APR
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR APR
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR APR

                if (_randomValueTemp <= 12) { _temperature = Temperature.Warm; }    //Wetter/Rainfall (60-40)       //TEMPERATURE UPDATE - FOR EACH DAY IN APR
                else { _temperature = Temperature.Cold; }

                //When Warm, the weather can either be Rainy or Cloudy with 60-40 ratio. When Cold, it can only be Rainy.
                if (_temperature == Temperature.Warm) { if (_randomValueWeather <= 18) { _weather = Weather.Rainy; } else { _weather = Weather.Cloudy; } }
                else if (_temperature == Temperature.Cold) { _weather = Weather.Rainy; }                           //WEATHER UPDATE - FOR EACH DAY IN APR

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 30)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    _climate = Climate.Muggy;                                                                       //CLIMATE UPDATE - FOR MAY
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR MAY
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR MAY
                }

            break;

            case Month.May://-------------------------------------------------------------------------MAY-----------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Wet; //Apr - Jun                                                                   //SEASON - FOR APR-JUN
                _climate = Climate.Muggy;                                                                           //CLIMATE - FOR MAY
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR MAY
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR MAY

                if (_randomValueTemp <= 16) { _temperature = Temperature.Hot; }     //Muggiest (80-20)              //TEMPERATURE UPDATE - FOR EACH DAY IN MAY
                else { _temperature = Temperature.Warm; }

                _weather = Weather.Cloudy;                                                                          //WEATHER UPDATE - FOR EACH DAY IN MAY

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    _climate = Climate.Warm;                                                                        //CLIMATE UPDATE - FOR JUN
                    _wind = WindStatus.Windier;                                                                     //WIND_STATUS UPDATE - FOR JUN
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR JUN
                }

            break;

            case Month.Jun://-------------------------------------------------------------------------JUNE----------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Wet; //Apr - Jun                                                                   //SEASON - FOR APR-JUN
                _climate = Climate.Warm;                                                                            //CLIMATE - FOR JUN
                _wind = WindStatus.Windier;                                                                         //WIND_STATUS - FOR JUN
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR JUN

                if (_randomValueTemp <= 12) { _temperature = Temperature.Warm; }    //After muggiest, so 60-40      //TEMPERATURE UPDATE - FOR EACH DAY IN JUN
                else { _temperature = Temperature.Hot; }

                //Cloudy or Clear can be Hot or Warm, so it's a straight 60-40.
                if (_randomValueWeather <= 18) { _weather = Weather.Cloudy; }
                else { _weather = Weather.Clear; }                                                                  //WEATHER UPDATE - FOR EACH DAY IN JUN

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 30)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    //_season = Season.Dry; //Jul - Sep                                                               //SEASON UPDATE - WET
                    _climate = Climate.Warm;                                                                        //CLIMATE UPDATE - FOR JUL
                    _wind = WindStatus.Windier;                                                                     //WIND_STATUS UPDATE - FOR JUL
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR JUL
                }

            break;

            case Month.Jul://-------------------------------------------------------------------------JULY----------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Jul - Sep                                                                   //SEASON - FOR JUL-SEP
                _climate = Climate.Warm;                                                                            //CLIMATE - FOR JUL
                _wind = WindStatus.Windier;                                                                         //WIND_STATUS - FOR JUL
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR JUL

                _temperature = Temperature.Warm;                                    //Best Time (100)               //TEMPERATURE UPDATE - FOR EACH DAY IN JUL

                //60-20-20
                if ( _randomValueWeather <= 18) { _weather = Weather.Comfortable; }
                else if(_randomValueWeather > 18 && _randomValueWeather <= 24) { _weather = Weather.Clear; }
                else { _weather = Weather.Cloudy; }                                                                 //WEATHER UPDATE - FOR EACH DAY IN JUL

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    _climate = Climate.Warm;                                                                        //CLIMATE UPDATE - FOR AUG
                    _wind = WindStatus.Windier;                                                                     //WIND_STATUS UPDATE - FOR AUG
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR AUG
                }

            break;

            case Month.Aug://-------------------------------------------------------------------------AUGUST--------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Jul - Sep                                                                   //SEASON - FOR JUL-SEP
                _climate = Climate.Warm;                                                                            //CLIMATE - FOR AUG
                _wind = WindStatus.Windier;                                                                         //WIND_STATUS - FOR AUG
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR AUG

                if (_randomValueTemp <= 16) { _temperature = Temperature.Warm; }    //Clearest (80-20)              //TEMPERATURE UPDATE - FOR EACH DAY IN AUG
                else { _temperature = Temperature.Hot; }

                //When Hot, its Sunny. When Warm, its either Comfortable or Clear with 50-50.
                if(_temperature == Temperature.Warm) { if (_randomValueWeather <= 15) { _weather = Weather.Comfortable; } else { _weather = Weather.Clear; } }
                else if(_temperature == Temperature.Hot) { _weather = Weather.Sunny; }                              //WEATHER UPDATE - FOR EACH DAY IN AUG

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    _climate = Climate.Muggy;                                                                       //CLIMATE UPDATE - FOR SEP
                    _wind = WindStatus.Windier;                                                                     //WIND_STATUS UPDATE - FOR SEP
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR SEP
                }

            break;

            case Month.Sep://-------------------------------------------------------------------------SEPTEMBER-----------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Jul - Sep                                                                   //SEASON - FOR JUL-SEP
                _climate = Climate.Muggy;                                                                           //CLIMATE - FOR SEP
                _wind = WindStatus.Windier;                                                                         //WIND_STATUS - FOR SEP
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR SEP

                if (_randomValueTemp <= 18) { _temperature = Temperature.Hot; }     //Hottest (90-10)               //TEMPERATURE UPDATE - FOR EACH DAY IN SEP
                else { _temperature = Temperature.Warm; }

                //When Hot, it is Sunny. When Warm, it is either Humid or Clear with 80-20.
                if (_temperature == Temperature.Warm) { if (_randomValueWeather <= 24) { _weather = Weather.Humid; } else { _weather = Weather.Clear; } }
                else if (_temperature == Temperature.Hot) { _weather = Weather.Sunny; }                              //WEATHER UPDATE - FOR EACH DAY IN SEP

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 30)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    //_season = Season.Wet; //Oct - Nov                                                               //SEASON UPDATE - WET
                    _climate = Climate.Muggy;                                                                       //CLIMATE UPDATE - FOR OCT
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR OCT
                    _windDirection = WindDirection.from_South;                                                      //WIND_DIRECTION UPDATE - FOR OCT
                }

            break;

            case Month.Oct://-------------------------------------------------------------------------OCTOBER-------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Wet; //Oct - Nov                                                                   //SEASON - FOR OCT-NOV
                _climate = Climate.Muggy;                                                                           //CLIMATE - FOR OCT
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR OCT
                _windDirection = WindDirection.from_South;                                                          //WIND_DIRECTION - FOR OCT

                if (_randomValueTemp <= 12) { _temperature = Temperature.Warm; }    //60-40                         //TEMPERATURE UPDATE - FOR EACH DAY IN OCT
                else { _temperature = Temperature.Hot; }

                //Cloudy or Clear can be Hot or Warm, so it's a straight 60-40.
                if (_randomValueWeather <= 18) { _weather = Weather.Cloudy; }
                else { _weather = Weather.Humid; }                                                                  //WEATHER UPDATE - FOR EACH DAY IN OCT

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    _climate = Climate.Overcast;                                                                    //CLIMATE UPDATE - FOR NOV
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR NOV
                    _windDirection = WindDirection.from_East;                                                       //WIND_DIRECTION UPDATE - FOR NOV
                }

            break;

            case Month.Nov://-------------------------------------------------------------------------NOVEMBER------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Wet; //Oct - Nov                                                                   //SEASON - FOR OCT-NOV
                _climate = Climate.Overcast;                                                                        //CLIMATE - FOR NOV
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR NOV
                _windDirection = WindDirection.from_East;                                                           //WIND_DIRECTION - FOR NOV

                if (_randomValueTemp <= 18) { _temperature = Temperature.Cold; }    //Coldest (90-10)               //TEMPERATURE UPDATE - FOR EACH DAY IN NOV
                else { _temperature = Temperature.Warm; }

                //When Warm, the weather can either be Rainy or Cloudy with 50-50 ratio, as it is the darkest and coldest. When Cold, it can only be Rainy.
                if (_temperature == Temperature.Warm) { if (_randomValueWeather <= 15) { _weather = Weather.Rainy; } else { _weather = Weather.Cloudy; } }
                else if (_temperature == Temperature.Cold) { _weather = Weather.Rainy; }                           //WEATHER UPDATE - FOR EACH DAY IN NOV

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 30)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth++;
                    _autoMonth = _customMonth;

                    //_season = Season.Dry; //Dec - Mar                                                               //SEASON UPDATE - DRY
                    _climate = Climate.Warm;                                                                        //CLIMATE UPDATE - FOR DEC
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR DEC
                    _windDirection = WindDirection.from_East;                                                       //WIND_DIRECTION UPDATE - FOR DEC
                }

            break;

            case Month.Dec://-------------------------------------------------------------------------DECEMBER------------------------------------------------
                if (_previousMonth != _customMonth) { _day = 1; _previousMonth = _customMonth; }

                _season = Season.Dry; //Dec - Mar                                                                   //SEASON - FOR DEC-MAR
                _climate = Climate.Warm;                                                                            //CLIMATE - FOR DEC
                _wind = WindStatus.Calmer;                                                                          //WIND_STATUS - FOR DEC
                _windDirection = WindDirection.from_East;                                                           //WIND_DIRECTION - FOR DEC

                if (_randomValueTemp <= 12) { _temperature = Temperature.Cold; }    //After coldest, so 60-40       //TEMPERATURE UPDATE - FOR EACH DAY IN DEC
                else { _temperature = Temperature.Warm; }

                //Cuz Rainy is Cold. So kept Cloudy as Warm.
                if (_temperature == Temperature.Cold) { _weather = Weather.Rainy; }
                else if(_temperature == Temperature.Warm) { _weather = Weather.Cloudy; }                            //WEATHER UPDATE - FOR EACH DAY IN DEC

                if (_autoMonth != _customMonth)
                {
                    #region EXPLANATION FOR HOURS, MINUTES, SECONDS SET TO 0 AND POTENTIAL FUTURE IMPROVEMENT PART
                    //Whenever there is custom change to month, the time will be set to morning 5 o'Clock [5AM].
                    //This is because there won't be any trouble to update weather data in Sky Manager. If this is not done - when time is 23:59 of a day and I change 
                    //the _customMonth, then the update weather might not generate data properly. However, for safety purpose, there are conditions implemented 
                    //in Sky Manager to overcome such scenarios as well. But still why to let it happen!
                    //Maybe later I can improve it so that such scenarios can be ruled out completely, where we don't have to keep safety conditions.
                    #endregion
                    _hours = 5;
                    _minutes = 0;
                    _seconds = 0;
                    _autoMonth = _customMonth;
                    SkyManager.Instance.UpdateWeather(true);
                }

                if (_day > 31)
                {
                    _day = 1;

                    _previousMonth = _customMonth;
                    _customMonth = Month.Jan;
                    _autoMonth = _customMonth;

                    _climate = Climate.Muggy;                                                                       //CLIMATE UPDATE - FOR JAN
                    _wind = WindStatus.Calmer;                                                                      //WIND_STATUS UPDATE - FOR JAN
                    _windDirection = WindDirection.from_East;                                                       //WIND_DIRECTION UPDATE - FOR JAN
                    _year++;                                                                                        //YEAR UPDATE
                }

            break;

            default://--------------------------------------------------------------------------------DEFAULT-------------------------------------------------
                
                //Debug.Log("Error in GameTimer - Try again with appropriate value!");

            break;
        }

        _dayOfWeek = (DayOfWeek)(PresentDayCount(this) % 7);
        PresentDayTimeInSeconds();

        _daysPassed = DaysPassed(this);
        _hoursPassed = HoursPassed(this);
        _minutesPassed = MinutesPassed(this);
    }

    private PartOfTheDay SetPartOfTheDayWithHoursValue(int hours)
    {
        //Sets the part of the day by hours value...
        if (hours >= 3 && hours <= 6)
        {
            return PartOfTheDay.EarlyMorning;
        }
        else if (hours >= 7 && hours <= 11)
        {
            return PartOfTheDay.Morning;
        }
        else if (hours == 12)
        {
            return PartOfTheDay.Noon;
        }
        else if (hours >= 13 && hours <= 16)
        {
            return PartOfTheDay.Afternoon;
        }
        else if (hours >= 17 && hours <= 19)
        {
            return PartOfTheDay.Evening;
        }
        else if (hours >= 20 && hours <= 23)
        {
            return PartOfTheDay.Night;
        }
        else if (hours >= 0 && hours <= 2)
        {
            return PartOfTheDay.MidNight;
        }
        else //This part won't run as all the conditions are satisfied by the above if else statements...
        {
            return PartOfTheDay.Morning;
        }
    }

    #endregion

    #region Internal Game Time Conversions
    private static float MinutesToSeconds(float minutes)
    {
        //Minutes * one minute in seconds
        return minutes * 60;
    }

    private static float HoursToMinutes(float hours)
    {
        //hours * one hour in minutes
        return hours * 60;
    }

    private static float DaysToHours(float days)
    {
        //Minutes * one minute in seconds
        return days * 24;
    }

    private static int MonthsToDays(Month month, int year = 0)
    {
        int days = 0;
        if(month == Month.Jan)
        {
            //month = Month.Dec;
            return 0;
        }
        else
        {
            month--;
        }
        
        switch (month)
        {
            case Month.Jan:
                days = 31;
                break;
            case Month.Feb:
                if(year != 0 && year % 4 == 0) //leap year
                {
                    days = 31 + 29;
                }
                else
                {
                    days = 31 + 28;
                }
                break;
            case Month.Mar:
                days = 59 + 31;
                break;
            case Month.Apr:
                days = 90 + 30;
                break;
            case Month.May:
                days = 120 + 31;
                break;
            case Month.Jun:
                days = 151 + 30;
                break;
            case Month.Jul:
                days = 181 + 31;
                break;
            case Month.Aug:
                days = 212 + 31;
                break;
            case Month.Sep:
                days = 243 + 30;
                break;
            case Month.Oct:
                days = 273 + 31;
                break;
            case Month.Nov:
                days = 304 + 30;
                break;
            case Month.Dec:
                days = 334 + 31;
                break;
        }
        return days;
    }

    private static int YearsToDays(int year)
    {
        int beginYear = 1940; //This is a leap year so, 365 + 1 days
        int noOfYears = year - beginYear;
        int noOfLeapYears = 0;
        if (year <= 1940)
        {
            return 0;
        }
        else
        {
            noOfLeapYears = (int)(noOfYears / 4) + 1;
        }
        return (noOfYears * 365) + noOfLeapYears; //if 1942, it will return 1940-1941, 1941-1942
    }

    //Returns current time of a day in seconds. Takes in hours, minutes and seconds for calculation.
    private float PresentDayTimeInSeconds()
    {
        float currentTime = MinutesToSeconds(HoursToMinutes(_hours)) + MinutesToSeconds(_minutes) + _seconds;
        return currentTime;
    }

    //Returns number of days passed + present day in count.
    private int PresentDayCount(GameTimer gameTimer)
    {
        int presentDayCount = YearsToDays(gameTimer._year) + MonthsToDays(gameTimer._customMonth, gameTimer._year) + gameTimer._day;
        return presentDayCount;
    }

    //Returns Minutes passed in total.
    private float MinutesPassed(GameTimer gameTimer)
    {
        float minutesPassed = HoursToMinutes(DaysToHours(DaysPassed(gameTimer)) + gameTimer._hours) + gameTimer._minutes;
        return minutesPassed;
    }

    //Returns hours passed in total.
    private float HoursPassed(GameTimer gameTimer)
    {
        float hoursPassed = DaysToHours(DaysPassed(gameTimer)) + gameTimer._hours;
        return hoursPassed;
    }

    //Returns days passed in total.
    private float DaysPassed(GameTimer gameTimer)
    {
        float daysPassed = YearsToDays(gameTimer._year) + MonthsToDays(gameTimer._customMonth, gameTimer._year) + (gameTimer._day - 1); //(gameTimer._day - 1) - so that only the passed days will be counted.
        return daysPassed;
    }
    #endregion
}