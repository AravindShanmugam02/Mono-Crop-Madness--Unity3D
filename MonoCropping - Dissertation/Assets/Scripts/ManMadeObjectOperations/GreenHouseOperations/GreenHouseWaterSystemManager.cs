using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHouseWaterSystemManager : MonoBehaviour
{
    public int GetCurrentWaterLevel() { return _currentWaterLevel; }
    public bool CheckIfWaterLevelIsSufficient(int quantity) { return CheckIfWaterLevelCanBeDeducted(quantity); }
    public int CheckHowMuchWaterLevelCanBeRefilled() { return CheckHowMuchWaterCanBeAdded(); }
    public void AddWaterToWaterSystem(int quantity) { IncreaseWaterLevel(quantity); }
    public void MinusWaterFromWaterSystem(int quantity) { DeductWaterLevel(quantity); }

    [SerializeField] private int _currentWaterLevel;
    [SerializeField] private int _maxCapacityOfGreenHouseWaterSystem;

    // Start is called before the first frame update
    void Start()
    {
        _currentWaterLevel = 0;
        _maxCapacityOfGreenHouseWaterSystem = 20;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void IncreaseWaterLevel(int quantity)
    {
        _currentWaterLevel += quantity;

        if(_currentWaterLevel > _maxCapacityOfGreenHouseWaterSystem) //Just a threshold check. It will not be used 99.9% or even 100%. We have dedicated methods to check if water level can be increased.
        {
            _currentWaterLevel = _maxCapacityOfGreenHouseWaterSystem;
        }
    }

    private void DeductWaterLevel(int quantity)
    {
        _currentWaterLevel -= quantity;

        if (_currentWaterLevel < 0) //Just a threshold check. It will not be used 99.9% or even 100%. However we have dedicated methods to check if water level can be deducted.
        {
            _currentWaterLevel = 0;
        }
    }

    private bool CheckIfWaterLevelCanBeDeducted(int quantity)
    {
        if((_currentWaterLevel - quantity) < 0)
        {
            return false;
        }

        return true;
    }

    private int CheckHowMuchWaterCanBeAdded()
    {
        return _maxCapacityOfGreenHouseWaterSystem - _currentWaterLevel;
    }
}
