using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public void AddAsITimerListener(ITimer listener) { RegisterAsITimerListener(listener); }
    public void RemoveFromITimerListener(ITimer listener) { UnregisterFromITimerListeners(listener); }
    public string GetCurrentSeason() { return _gameTimer.GetSeason().ToString(); }
    public string GetCurrentPartOfTheDay() { return _gameTimer.GetPartOfTheDay().ToString(); }
    public int GetCurrentHour() { return _gameTimer.GetHours(); }
    public int GetCurrentMinute() { return _gameTimer.GetMinutes(); }
    public int GetCurrentDayOrDate() { return _gameTimer.GetDate(); }
    
    public void SetGameSlotToLoad(GameManager.GameSlotsToLoad gameSlotToLoad) { _gameSlotToLoad = gameSlotToLoad; }
    public void SetPlayerSleepingStatus(bool toggle) { _isPlayerSleeping = toggle; }

    private struct GamePlayTimeSlots
    {
        public int yearToLoad;
        public GameTimer.Season seasonToLoad;
        public GameTimer.Month monthToLoad;
        public int dayToLoad;
        public GameTimer.DayOfWeek dayOfWeekToLoad;
        public int hoursToLoad;
        public int minutesToLoad;
        public int secondsToLoad;
    }
    GamePlayTimeSlots slot1Jan;
    GamePlayTimeSlots slot2Apr;


    //To Store The Slot To Load...
    [SerializeField] private GameManager.GameSlotsToLoad _gameSlotToLoad;

    [Header("Game Time Scaler")]
    [SerializeField] private float _timeScale = 1.0f;

    //Makes the whole GameTimer class as a serializefield provided GameTimer class is classified as Serializable.
    [Header("Game Time")]
    [SerializeField] private GameTimer _gameTimer;

    //ITimer Interface List
    List<ITimer> _itimerListenersList;

    //Random Generator Handle
    private System.Random _randHandle;

    //To handle player sleep...
    private bool _isPlayerSleeping;

    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static TimeManager Instance { get; private set; }

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

        _isPlayerSleeping = false;
        _itimerListenersList = new List<ITimer>();

        //Slot1Jan
        slot1Jan = new GamePlayTimeSlots();
        slot1Jan.yearToLoad = 1940;
        slot1Jan.seasonToLoad = GameTimer.Season.Dry;
        slot1Jan.monthToLoad = GameTimer.Month.Jan;
        slot1Jan.dayToLoad = 01;
        slot1Jan.dayOfWeekToLoad = GameTimer.DayOfWeek.Mon;
        slot1Jan.hoursToLoad = 03;
        slot1Jan.minutesToLoad = 00;
        slot1Jan.secondsToLoad = 00;

        //Slot2Apr
        slot2Apr = new GamePlayTimeSlots();
        slot2Apr.yearToLoad = 1940;
        slot2Apr.seasonToLoad = GameTimer.Season.Wet;
        slot2Apr.monthToLoad = GameTimer.Month.Apr;
        slot2Apr.dayToLoad = 01;
        slot2Apr.dayOfWeekToLoad = GameTimer.DayOfWeek.Mon;
        slot2Apr.hoursToLoad = 03;
        slot2Apr.minutesToLoad = 00;
        slot2Apr.secondsToLoad = 00;
    }

    // Start is called before the first frame update
    void Start()
    {
        _randHandle = new System.Random();

        if (_gameSlotToLoad == GameManager.GameSlotsToLoad.Slot1)
        {
            GamePlayTimeSlotToLoad(slot1Jan);
        }
        else if(_gameSlotToLoad == GameManager.GameSlotsToLoad.Slot2)
        {
            GamePlayTimeSlotToLoad(slot2Apr);
        }

        StartCoroutine(GameTimeUpdate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GamePlayTimeSlotToLoad(GamePlayTimeSlots slot)
    {
        _gameTimer = new GameTimer(slot.yearToLoad, slot.seasonToLoad, slot.monthToLoad, slot.dayToLoad, 
            slot.dayOfWeekToLoad, slot.hoursToLoad, slot.minutesToLoad, slot.secondsToLoad);
    }

    //Handles the coroutine for calling GameTicks for every second.
    IEnumerator GameTimeUpdate()
    {
        //yield return new WaitForSeconds(1f); //Just to give one second delayed start.

        while (true)
        {
            if (Input.GetKey(KeyCode.P) || _isPlayerSleeping)
            {
                //yield return new WaitForSeconds(0.000000000000001f); //for testing
                yield return new WaitForEndOfFrame(); //for testing
            }
            else
            {
                yield return new WaitForSeconds(1 / _timeScale);
            }

            GameTicks();
        }
    }

    //Called every second to handle the game time.
    void GameTicks()
    {
        _gameTimer.UpdateTime();

        foreach(ITimer l in _itimerListenersList)
        {
            l.TickUpdate(_gameTimer);
        }
    }

    //Handling registering as listener.
    void RegisterAsITimerListener(ITimer listener)
    {
        _itimerListenersList.Add(listener);
    }

    //Handling Unregistering from listeners.
    void UnregisterFromITimerListeners(ITimer listener)
    {
        _itimerListenersList.Remove(listener);
    }

    //Returns a new instance created from a copy constructor which gives a specific time value (not just a reference) for every object that calls this.
    //Example to understand a copy constructor: https://www.geeksforgeeks.org/c-sharp-copy-constructor/
    public GameTimer GetCurrentTimeStamp(GameTimer gameTimer)
    {
        return new GameTimer(gameTimer);
    }

    //Returns the difference between two time stamps in hours... (Causes a potential issue when difference is required accurately in muinutes, because this always tells difference in round hours)
    public float DifferenceInTimeUsingHours(GameTimer ownTimer, GameTimer gameTimer)
    {
        float ownTimerInHours = ownTimer.GetNoOfHoursPassed(ownTimer);
        float gameTimerInHours = gameTimer.GetNoOfHoursPassed(gameTimer);
        return Mathf.Abs(gameTimerInHours - ownTimerInHours);
    }

    //Returns the difference between two time stamps in Minutes...
    public float DifferenceInTimeUsingMinutes(GameTimer ownTimer, GameTimer gameTimer)
    {
        float ownTimerInMinutes = ownTimer.GetNoOfMinutesPassed(ownTimer);
        float gameTimerInMinutes = gameTimer.GetNoOfMinutesPassed(gameTimer);
        return Mathf.Abs(gameTimerInMinutes - ownTimerInMinutes);
    }

    //Returns Days to Minutes...
    public float GetDaysInMinutes(float days)
    {
        return (_gameTimer.GetHoursToMinutes(_gameTimer.GetDaysToHours(days)));
    }

    //Returns Hours to Minutes...
    public float GetHoursInMinutes(float hours)
    {
        return (_gameTimer.GetHoursToMinutes(hours));
    }

    public float GetRandomMinuteInAnHour()
    {
        return (_randHandle.Next(0, 59));
    }

    public float GetRandomHourInADay()
    {
        return (_randHandle.Next(0, 23));
    }

    public float GetRandomHourInADayForSwitchedMonth()
    {
        //It fetches hours from current hour + 1, so that it doesn't miss to generate rain data because of minute mismatch in
        //GetRandomMinuteInAnHour() as we don't take it from current minute..
        if (_gameTimer.GetHours() < 22)
        {
            return (_randHandle.Next(_gameTimer.GetHours() + 1, 23));
        }
        else
        {
            return _gameTimer.GetHours();
        }
    }

    public float GetRandomMinuteInAnHourForSwitchedMonth()
    {
        //Kept 55 for safety reasons... since 1 real world second is equal to 1 in-game minute. This update happens once a real world second,
        //So Rain data shouldn't be missed by keeping close numbers like 57 - 59...
        if (_gameTimer.GetMinutes() < 55)
        {
            return (_randHandle.Next(_gameTimer.GetMinutes() + 1, 59));
        }
        else
        {
            return _gameTimer.GetMinutes();
        }
    }

    public float GetRandomDurationInADay()
    {
        return (_randHandle.Next(5, 10)); //Kept 5 - 10, so that it gives more probability for the land to become clay state.
    }
}