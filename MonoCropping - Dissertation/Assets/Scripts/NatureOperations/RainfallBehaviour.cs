using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainfallBehaviour : MonoBehaviour, ITimer
{
    [SerializeField]
    private float _duration, EstHourOfStop;

    private GameTimer _ownTimer, _gameTimer;

    private bool isObjectAlive;

    // Start is called before the first frame update
    void Start()
    {
        isObjectAlive = true;
        if (isObjectAlive == true)
        {
            TimeManager.Instance.AddAsITimerListener(this);
            CalculateSpan();
            SkyManager.Instance.SetRainStatus(true);
            SkyManager.Instance.SetRainfallDuration(_duration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectAlive == true)
        {
            if(_ownTimer == null && _gameTimer != null)
            {
                _ownTimer = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);

                EstHourOfStop = _duration + _ownTimer.GetHours(); // 6 + 20 = 26

                if(EstHourOfStop > 24) // 26 > 24
                {
                    EstHourOfStop -= 24; // 26 - 24 = 2
                }
            }
            if (_ownTimer != null && _gameTimer != null)
            {
                if (TimeManager.Instance.DifferenceInTimeUsingMinutes(_ownTimer, _gameTimer) >= 360) // 6 hours in minutes
                {
                    SkyManager.Instance.SetIsRainingFor6Hours(true);
                }

                isTimeToDestroy();
            }
        }
    }

    void CalculateSpan()
    {
        _duration = TimeManager.Instance.GetRandomDurationInADay();
    }

    void isTimeToDestroy()
    {
        if (_gameTimer.GetHours() == EstHourOfStop)
        {
            isObjectAlive = false;
            TimeManager.Instance.RemoveFromITimerListener(this);
            SkyManager.Instance.SetIsRainingFor6Hours(false);
            SkyManager.Instance.SetRainStatus(false);
            Destroy(this.gameObject);
        }
    }

    void ITimer.TickUpdate(GameTimer gameTimer) //Updates every frame
    {
        if(isObjectAlive == true)
        {
            if (_gameTimer == null) { _gameTimer = gameTimer; }
        }
    }
}